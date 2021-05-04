namespace Craiel.UnityEssentials.Runtime.Resource
{
    using System;
    using Contracts;
    using UnityEngine;
    using UnityEngine.Networking;

    public class ResourceStreamRequest : IResourceRequest, IDisposable
    {
        private readonly UnityWebRequest stream;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ResourceStreamRequest(ResourceLoadInfo info)
        {
            this.Info = info;
            
            this.stream = UnityWebRequest.Get(info.Key.Path);
            this.stream.SendWebRequest();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public ResourceLoadInfo Info { get; private set; }
        
        public bool IsDone
        {
            get
            {
                return this.stream.isDone;
            }
        }

        public byte[] GetData()
        {
            Debug.Assert(this.IsDone);

            if (!string.IsNullOrEmpty(this.stream.error))
            {
                EssentialsCore.Logger.Error("ResourceStreamRequest had errors: {0}", this.stream.error);
                return null;
            }

            return this.stream.downloadHandler.data;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                this.stream.Dispose();
            }
        }
    }
}
