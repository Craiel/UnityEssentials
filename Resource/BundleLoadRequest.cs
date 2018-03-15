﻿namespace Craiel.UnityEssentials.Resource
{
    using System.IO;
    using Enums;
    using IO;
    using UnityEngine;

    public class BundleLoadRequest
    {
        private const int BytesReadPerFrame = 256 * 256 * 32; // 512k

        private byte[] data;
        private FileStream stream;
        private bool fileLoaded;
        private int dataOffset;
        
        private AssetBundleCreateRequest request;

        private AssetBundle bundle;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public BundleLoadRequest(BundleLoadInfo info, ManagedFile file)
        {
            this.Info = info;
            this.File = file;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public BundleLoadInfo Info { get; private set; }

        public ManagedFile File { get; private set; }
        
        public void LoadImmediate()
        {
            if ((this.Info.Flags & BundleLoadFlags.Uncompressed) != 0)
            {
                this.LoadFromFileImmediate();
                return;
            }

            this.LoadFromMemoryImmediate();
        }

        public bool ContinueLoading()
        {
            if ((this.Info.Flags & BundleLoadFlags.Uncompressed) != 0)
            {
                // File is not async for now, just load sync
                this.LoadFromFileImmediate();
                return false;
            }

            if (this.ContinueReadingFile())
            {
                return true;
            }

            if (this.request == null)
            {
#if UNITY_5_2
                this.request = AssetBundle.CreateFromMemory(this.data);
#else
                this.request = AssetBundle.LoadFromMemoryAsync(this.data);
#endif
                return true;
            }

            if (!this.request.isDone)
            {
                return true;
            }

            this.bundle = this.request.assetBundle;
            this.request = null;
            
            // Clear out the data to have it collected by the GC
            if (this.stream != null)
            {
                this.stream.Dispose();
                this.stream = null;
            }

            this.data = null;
            return false;
        }

        public AssetBundle GetBundle()
        {
            return this.bundle;
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private bool ContinueReadingFile()
        {
            if (this.fileLoaded)
            {
                return false;
            }

            if (this.stream == null)
            {
                this.stream = this.File.OpenRead();
                this.data = new byte[this.stream.Length];
                return true;
            }

            if (this.stream.CanRead && this.dataOffset < this.data.Length)
            {
                int bytesRead = this.stream.Read(
                    this.data,
                    this.dataOffset,
                    Mathf.Min(BytesReadPerFrame, this.data.Length - this.dataOffset));
                this.dataOffset += bytesRead;

                return true;
            }

            this.fileLoaded = true;
            return false;
        }

        private void LoadFromFileImmediate()
        {
#if UNITY_5_2
            this.bundle = AssetBundle.CreateFromFile(this.File.GetPath());
#else
            this.bundle = AssetBundle.LoadFromFile(this.File.GetPath());
#endif
        }

        private void LoadFromMemoryImmediate()
        {
            using (this.stream = this.File.OpenRead())
            {
                this.data = new byte[this.stream.Length];
                this.stream.Read(this.data, 0, this.data.Length);
            }

#if UNITY_5_2
            this.bundle = AssetBundle.CreateFromMemoryImmediate(this.data);
#else
            this.bundle = AssetBundle.LoadFromMemory(this.data);
#endif

            this.stream = null;
            this.data = null;
        }
    }
}
