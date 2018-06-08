﻿namespace Craiel.UnityEssentials.Runtime.Resource
{
    using System.Collections.Generic;
    using System.Threading;
    using Enums;
    using NLog;
    using Singletons;

    // Made to load resources from Application.streamingAssetsPath and other WWW accessible places
    public class ResourceStreamProvider : UnitySingleton<ResourceStreamProvider>
    {
        private const int DefaultRequestPoolSize = 30;

        private const float DefaultReadTimeout = 10;

        private const int MaxConsecutiveSyncCallsInAsync = 20;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ResourceMap<byte[]> resourceMap;

        private readonly Queue<ResourceLoadInfo> currentPendingLoads;

        private readonly ResourceRequestPool<ResourceStreamRequest> requestPool;

        private readonly IDictionary<ResourceKey, long> history;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ResourceStreamProvider()
        {
            this.resourceMap = new ResourceMap<byte[]>();

            this.currentPendingLoads = new Queue<ResourceLoadInfo>();

            this.requestPool = new ResourceRequestPool<ResourceStreamRequest>(DefaultRequestPoolSize);

            this.history = new Dictionary<ResourceKey, long>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public event OnResourceLoadingDelegate ResourceLoading;
        public event OnResourceLoadedDelegate ResourceLoaded;

        public int PendingForLoad
        {
            get
            {
                return this.currentPendingLoads.Count;
            }
        }

        public int ResourcesLoaded { get; private set; }

        public bool EnableHistory { get; set; }

        public IDictionary<ResourceKey, long> GetHistory()
        {
            return this.history;
        }

        public void RegisterResource(ResourceKey key, ResourceLoadFlags flags = ResourceLoadFlags.Cache)
        {
            if ((flags & ResourceLoadFlags.Cache) != 0)
            {
                // Cache the resource in the map
                this.resourceMap.RegisterResource(key);
            }

            lock (this.currentPendingLoads)
            {
                this.currentPendingLoads.Enqueue(new ResourceLoadInfo(key, flags));
            }
        }

        public void UnregisterResource(ResourceKey key)
        {
            this.resourceMap.UnregisterResource(key);
        }

        public byte[] AcquireOrLoadResource(ResourceKey key, ResourceLoadFlags flags = ResourceLoadFlags.Cache)
        {
            byte[] data = this.resourceMap.GetData(key);
            if (data == null)
            {
                if (key.Bundle != null)
                {
                    Logger.Error("Can not stream bundled resources!");
                    return null;
                }

                this.DoLoadImmediate(new ResourceLoadInfo(key, flags));
                data = this.resourceMap.GetData(key);
                if (data == null)
                {
                    Logger.Error("Could not load resource on-demand");
                    return null;
                }
            }

            return data;
        }

        public byte[] AcquireResource(ResourceKey key)
        {
            byte[] data = this.resourceMap.GetData(key);
            if (data == null)
            {
                Logger.Error("Resource was not loaded or registered: {0}", key);
                return null;
            }

            return data;
        }

        public string AcquireStringResource(ResourceKey key)
        {
            byte[] data = this.AcquireResource(key);
            if (data == null)
            {
                return null;
            }
            
            if (this.TextContainsBOM(data))
            {
                return System.Text.Encoding.UTF8.GetString(data, 3, data.Length - 3);
            }

            return System.Text.Encoding.UTF8.GetString(data, 0, data.Length);
        }

        public bool TryAcquireResource(ResourceKey key, out byte[] data)
        {
            data = this.resourceMap.GetData(key);
            return data != null;
        }
        
        public void ClearCache(ResourceKey key)
        {
            if (this.resourceMap.HasData(key))
            {
                this.resourceMap.SetData(key, null);
            }
        }

        public bool ContinueLoad()
        {
            IList<ResourceStreamRequest> finishedRequests = this.requestPool.GetFinishedRequests();
            if (finishedRequests != null)
            {
                foreach (ResourceStreamRequest request in finishedRequests)
                {
                    byte[] result = request.GetData();
                    if (result != null)
                    {
                        this.FinalizeLoadResource(request.Info, result);
                    }
                    else
                    {
                        Logger.Warn("Load of {0} returned unexpected no data", request.Info.Key);
                    }
                }
            }

            int consecutiveSyncCalls = 0;
            while (this.currentPendingLoads.Count > 0 && this.requestPool.HasFreeSlot())
            {
                ResourceLoadInfo info = this.currentPendingLoads.Dequeue();

                if (this.resourceMap.HasData(info.Key))
                {
                    // This resource was already loaded, continue
                    return true;
                }

                if ((info.Flags & ResourceLoadFlags.Sync) != 0)
                {
                    // This resource is a forced sync load
                    this.DoLoadImmediate(info);
                    consecutiveSyncCalls++;

                    if (consecutiveSyncCalls > MaxConsecutiveSyncCallsInAsync)
                    {
                        // Give a frame for the UI to catch up, we probably load a lot of tiny resources
                        return true;
                    }

                    continue;
                }

                if (this.ResourceLoading != null)
                {
                    this.ResourceLoading(info);
                }

                this.requestPool.AddRequest(new ResourceStreamRequest(info));
            }
            
            return this.currentPendingLoads.Count > 0 || this.requestPool.HasPendingRequests();
        }

        public void LoadImmediate()
        {
            if (this.currentPendingLoads.Count <= 0)
            {
                return;
            }
            
            int resourceCount = this.currentPendingLoads.Count;
            while (this.currentPendingLoads.Count > 0)
            {
                ResourceLoadInfo info = this.currentPendingLoads.Dequeue();
                if (!this.resourceMap.HasData(info.Key))
                {
                    this.DoLoadImmediate(info);
                }
            }
            
            Logger.Info("Immediate! Loaded {0} resources in {1}ms", resourceCount, -1);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void DoLoadImmediate(ResourceLoadInfo info)
        {
            if (this.ResourceLoading != null)
            {
                this.ResourceLoading(info);
            }
            
            var request = new ResourceStreamRequest(info);
            float time = UnityEngine.Time.time;
            while (!request.IsDone)
            {
                Thread.Sleep(2);
                if (UnityEngine.Time.time > time + DefaultReadTimeout)
                {
                    Logger.Error("Timeout while reading {0}", info.Key);
                    return;
                }
            }
            
            this.FinalizeLoadResource(info, request.GetData());
        }

        private void FinalizeLoadResource(ResourceLoadInfo info, byte[] data)
        {
            if (data == null)
            {
                Logger.Warn("Loading {0} returned null data", info.Key);
                return;
            }

            if (this.EnableHistory)
            {
                if (this.history.ContainsKey(info.Key))
                {
                    this.history[info.Key] += 1;
                }
                else
                {
                    this.history.Add(info.Key, 1);
                }
            }

            this.resourceMap.SetData(info.Key, data);

            this.ResourcesLoaded++;
            if (this.ResourceLoaded != null)
            {
                this.ResourceLoaded(info, 1);
            }
        }

        private bool TextContainsBOM(byte[] textData)
        {
            // Getting text through WWW will sometimes return the data with BOM
            // 239 187 191 (EF BB BF)
            if (textData.Length < 3)
            {
                return false;
            }

            return textData[0] == 239 && textData[1] == 187 && textData[2] == 191;
        }
    }
}
