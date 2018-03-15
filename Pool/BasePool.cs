namespace Craiel.UnityEssentials.Pool
{
    using System;
    using System.Collections.Generic;
    using Contracts;

    /// <summary>
    /// A pool of objects that can be reused to avoid allocation
    /// </summary>
    /// <typeparam name="T">the object to pool</typeparam>
    public abstract class BasePool<T>
        where T : class, IPoolable
    {
        private readonly Stack<T> freeObjects;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------

        /// <summary>
        /// Creates a new <see cref="BasePool{T}"/> instance
        /// </summary>
        /// <param name="initialCapacity">initial capacity of the pool</param>
        /// <param name="maxCapacity">The maximum number of free objects to store in this pool</param>
        protected BasePool(int initialCapacity = 16, int maxCapacity = int.MaxValue)
        {
            this.freeObjects = new Stack<T>(initialCapacity);
            this.Max = maxCapacity;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public int Max { get; private set; }

        public int Peak { get; private set; }

        /// <summary>
        /// The object may be new (from <see cref="NewObject"/>) or reused
        /// </summary>
        /// <returns>Returns an object from this pool</returns>
        public virtual T Obtain()
        {
            return this.freeObjects.Count == 0 ? this.NewObject() : this.freeObjects.Pop();
        }

        /// <summary>
        /// Puts the specified object in the pool, making it eligible to be returned by <see cref="Obtain"/>.
        /// If the pool already contains <see cref="Max"/> free objects, the specified object is reset but not added to the pool.
        /// </summary>
        /// <param name="entry">the entry to release</param>
        public void Free(T entry)
        {
            if (entry == null)
            {
                throw new ArgumentException("object cannot be null.");
            }

            if (this.freeObjects.Count < this.Max)
            {
                this.freeObjects.Push(entry);
                this.Peak = Math.Max(this.Peak, this.freeObjects.Count);
            }

            this.Reset(entry);
        }

        /// <summary>
        /// Puts the specified objects in the pool. Null objects within the array are silently ignored
        /// </summary>
        public void FreeAll(T[] objects)
        {
            if (objects == null)
            {
                throw new ArgumentException("objects cannot be null.");
            }
            
            int max = this.Max;
            for (int i = 0; i < objects.Length; i++)
            {
                T entry = objects[i];
                if (entry == null)
                {
                    continue;
                }

                if (this.freeObjects.Count < max)
                {
                    this.freeObjects.Push(entry);
                }

                this.Reset(entry);
            }

            this.Peak = Math.Max(this.Peak, this.freeObjects.Count);
        }

        /// <summary>
        /// Removes all free objects from this pool
        /// </summary>
        public void Clear()
        {
            this.freeObjects.Clear();
        }

        /// <summary>
        /// The number of objects available to be obtained
        /// </summary>
        /// <returns></returns>
        public int GetFree()
        {
            return this.freeObjects.Count;
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------

        /// <summary>
        /// Called when an object is freed to clear the state of the object for possible later reuse. 
        /// The default implementation calls <see cref="IPoolable.Reset"/> if the object is <see cref="IPoolable"/>.
        /// </summary>
        /// <param name="entry">the instance to reset</param>
        protected virtual void Reset(T entry)
        {
            entry.Reset();
        }

        protected abstract T NewObject();
    }
}
