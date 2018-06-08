namespace Craiel.UnityEssentials.Runtime.Pool
{
    using System;
    using Contracts;

    public abstract class TrackedPool<T> : BasePool<T>
        where T : class, IPoolable
    {
        private const int DefaultSize = 20;

        private T[] activeEntries;

        private int nextFreeSlot;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        protected TrackedPool(int capacity = DefaultSize)
            : base(initialCapacity: capacity)
        {
            this.activeEntries = new T[capacity];
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public int ActiveCount { get; private set; }

        public void Update()
        {
            for (var i = 0; i < this.activeEntries.Length; i++)
            {
                if (this.activeEntries[i] == null)
                {
                    continue;
                }

                if (!this.UpdateActiveEntry(this.activeEntries[i]))
                {
                    this.Free(this.activeEntries[i]);
                    this.activeEntries[i] = null;
                    this.ActiveCount--;

                    if (this.nextFreeSlot > i)
                    {
                        this.nextFreeSlot = i;
                    }
                }
            }
        }

        public override T Obtain()
        {
            T entry = base.Obtain();
            this.activeEntries[this.nextFreeSlot] = entry;
            this.FindNextFreeSlot();
            this.ActiveCount++;

            return entry;
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void Reset(T entry)
        {
            for (var i = 0; i < this.activeEntries.Length; i++)
            {
                if (this.activeEntries[i] != null && this.activeEntries[i] == entry)
                {
                    this.activeEntries[i] = null;
                    this.ActiveCount--;

                    if (this.nextFreeSlot > i)
                    {
                        this.nextFreeSlot = i;
                    }
                    
                    break;
                }
            }

            base.Reset(entry);
        }

        protected abstract bool UpdateActiveEntry(T entry);

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void FindNextFreeSlot()
        {
            for (var i = this.nextFreeSlot; i < this.activeEntries.Length; i++)
            {
                if (this.activeEntries[i] == null)
                {
                    this.nextFreeSlot = i;
                    return;
                }
            }

            this.nextFreeSlot = this.activeEntries.Length;
            Array.Resize(ref this.activeEntries, this.activeEntries.Length + DefaultSize);
        }
    }
}
