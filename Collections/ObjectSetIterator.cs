namespace Craiel.UnityEssentials.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class ObjectSetIterator<T> : IEnumerable<T>, IEnumerator<T>
    {
        private readonly ObjectSet<T> set;

        private int nextIndex;
        private int currentIndex;

        private bool hasNext;
        private bool valid = true;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public ObjectSetIterator(ObjectSet<T> set)
        {
            this.set = set;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool HasNext
        {
            get
            {
                if (!this.valid)
                {
                    throw new InvalidOperationException("#iterator() cannot be used nested.");
                }

                return this.hasNext;
            }
        }

        public T Current
        {
            get
            {
                return this.set.keyTable[this.currentIndex];
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return this.set.keyTable[this.currentIndex];
            }
        }

        public void Reset()
        {
            this.currentIndex = -1;
            this.nextIndex = -1;
            this.FindNextIndex();
        }

        public void Remove()
        {
            if (this.currentIndex < 0)
            {
                throw new IllegalStateException("next must be called before remove.");
            }

            if (this.currentIndex >= this.set.capacity)
            {
                this.set.RemoveStashIndex(this.currentIndex);
                this.nextIndex = this.currentIndex - 1;
                this.FindNextIndex();
            }
            else
            {
                this.set.keyTable[this.currentIndex] = default(T);
            }

            this.currentIndex = -1;
            this.set.Size--;
        }

        public bool MoveNext()
        {
            if (!this.hasNext)
            {
                return false;
            }

            if (!this.valid)
            {
                throw new InvalidOperationException("#iterator() cannot be used nested.");
            }
            
            this.currentIndex = this.nextIndex;
            this.FindNextIndex();
            return true;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this;
        }

        public void Dispose()
        {
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private void FindNextIndex()
        {
            this.hasNext = false;
            T[] keyTable = this.set.keyTable;
            for (int n = this.set.capacity + this.set.stashSize; ++this.nextIndex < n;)
            {
                if (keyTable[this.nextIndex] != null)
                {
                    this.hasNext = true;
                    break;
                }
            }
        }
    }
}
