namespace Craiel.UnityEssentials.Runtime.Pool
{
    using System;
    using Unity.Collections;
    using UnityEngine;
    using UnityEngine.Jobs;
    using Object = UnityEngine.Object;

    public class GameObjectPool : IDisposable
    {
        private readonly GameObject[] entries;
        private readonly Transform[] transforms;
        private readonly TransformAccessArray transformAccess;
        private readonly Renderer[] renderers;
        
        private readonly ushort count;
        private readonly GameObject root;
        
        private NativeArray<bool> alive;

        private ushort aliveCount;
        private ushort nextFreeObject;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        private GameObjectPool(ushort objectCount)
        {
            if (objectCount == 0 || objectCount == ushort.MaxValue)
            {
                throw new ArgumentException(nameof(objectCount));
            }
            
            this.entries = new GameObject[objectCount];
            this.transforms = new Transform[objectCount];
            this.renderers = new Renderer[objectCount];
            
            this.alive = new NativeArray<bool>(objectCount, Allocator.Persistent);
        }
        
        public GameObjectPool(ushort objectCount, GameObject prefab)
            : this(objectCount)
        {
            if (prefab == null)
            {
                throw new ArgumentException(nameof(prefab));
            }
            
            this.count = objectCount;
            this.root = new GameObject(string.Format("Prefab{0}:{1}", this.GetType().Name, prefab.name));
            for (ushort i = 0; i < this.count; i++)
            {
                this.Initialize(i, Object.Instantiate(prefab));
            }
            
            this.transformAccess = new TransformAccessArray(this.transforms);
        }

        public GameObjectPool(GameObject[] instances)
            : this((ushort)instances.Length)
        {
            if (instances.Length >= ushort.MaxValue)
            {
                throw new ArgumentException(nameof(instances));
            }
            
            this.count = (ushort)instances.Length;
            this.root = new GameObject(string.Format("Custom{0}", this.GetType().Name));
            for (ushort i = 0; i < this.count; i++)
            {
                this.Initialize(i, instances[i], true);
            }
            
            this.transformAccess = new TransformAccessArray(this.transforms);
        }
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public ushort Count => this.count;

        public ushort Alive => this.aliveCount;

        public TransformAccessArray TransformAccess => this.transformAccess;

        public virtual bool Obtain(out GameObject entry, out ushort id)
        {
            if (this.nextFreeObject == ushort.MaxValue)
            {
                entry = null;
                id = ushort.MaxValue;
                return false;
            }

            id = this.nextFreeObject;
            
            entry = this.entries[id];
            entry.SetActive(true);
            
            this.alive[id] = true;
            this.aliveCount++;
            
            this.FindNextFreeObject();

            return true;
        }

        public virtual void Release(ushort id)
        {
            if (id == ushort.MaxValue || !this.alive[id])
            {
                throw new InvalidOperationException("Release called with invalid ticket");
            }

            this.entries[id].SetActive(false);
            this.alive[id] = false;
            this.aliveCount--;

            if (this.nextFreeObject == ushort.MaxValue)
            {
                this.nextFreeObject = id;
            }
        }

        public bool IsAlive(ushort id)
        {
            return id != ushort.MaxValue && this.alive[id];
        }
        
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected virtual void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                this.transformAccess.Dispose();
                this.alive.Dispose();
            }
        }

        protected NativeArray<bool> GetAliveState()
        {
            return this.alive;
        }
        
        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void Initialize(ushort index, GameObject instance, bool reParent = false)
        {
            this.entries[index] = instance;
            if (reParent)
            {
                instance.transform.SetParent(this.root.transform);
            }
            
            instance.SetActive(false);
            
            this.transforms[index] = instance.transform;
            this.renderers[index] = instance.GetComponent<Renderer>();
        }
        
        private void FindNextFreeObject()
        {
            for (var n = 0; n < this.entries.Length; n++)
            {
                this.nextFreeObject++;
                if (this.nextFreeObject == this.entries.Length)
                {
                    // Loop around
                    this.nextFreeObject = 0;
                }

                if (!this.alive[this.nextFreeObject])
                {
                    return;
                }
            }

            // No free entry
            this.nextFreeObject = ushort.MaxValue;
        }
    }
}