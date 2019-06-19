namespace Craiel.UnityEssentials.Runtime.Pool
{
    using System;
    using Contracts;
    using Resource;
    using UnityEngine;

    public class GameObjectPool<T> : TrackedPool<T>
        where T : MonoBehaviour, IPoolable
    {
        private GameObject prefab;
        private Func<T, bool> activeUpdateCallback;
        
        private Transform root;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        private void Initialize(Func<T, bool> updateCallback, Transform poolRoot)
        {
            this.root = poolRoot;
            this.activeUpdateCallback = updateCallback;
        }
        
        public void Initialize(ResourceKey poolPrefabKey, Func<T, bool> updateCallback, Transform poolRoot = null)
        {
            this.Initialize(updateCallback, poolRoot);

            this.prefab = poolPrefabKey.LoadManaged<GameObject>();
            if (this.prefab == null)
            {
                throw new InvalidOperationException("Resource could not be loaded: " + poolPrefabKey);
            }
        }
        
        public void Initialize(GameObject poolPrefab, Func<T, bool> updateCallback, Transform poolRoot = null)
        {
            this.Initialize(updateCallback, poolRoot);

            this.prefab = poolPrefab;
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override T NewObject()
        {
            GameObject instance = UnityEngine.Object.Instantiate(this.prefab);

            if (this.root != null)
            {
                instance.transform.SetParent(this.root);
            }

            return instance.GetComponentInChildren<T>();
        }

        protected override bool UpdateActiveEntry(T entry)
        {
            return this.activeUpdateCallback(entry);
        }
    }
}
