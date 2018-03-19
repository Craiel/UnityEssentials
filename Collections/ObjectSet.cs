namespace Craiel.UnityEssentials.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using Exceptions;
    using Utils;

    /// <summary>
    /// An unordered set where the keys are objects. This implementation uses cuckoo hashing using 3 hashes, random walking, 
    /// and a small stash for problematic keys.Null keys are not allowed. No allocation is done except when growing the table size.
    /// <para>
    /// This set performs very fast contains and remove (typically O(1), worst case O(log(n))). 
    /// Add may be a bit slower, depending on hash collisions.Load factors greater than 0.91 greatly increase the chances the set will have to rehash to the next higher POT size.
    /// </para>
    /// </summary>
    /// <typeparam name="T">the type this set holds</typeparam>
    public class ObjectSet<T> : IEnumerable<T>
    {
        private const int Prime1 = unchecked((int)0xbe1f14b1);
        private const int Prime2 = unchecked((int)0xb4b82e39);
        private const int Prime3 = unchecked((int)0xced1c241);

        private readonly float loadFactor;

        private int hashShift;

        private int mask;

        private int threshold;

        private int stashCapacity;

        private int pushIterations;
        
        internal T[] keyTable;
        internal int capacity;
        internal int stashSize;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------

        /// <summary>
        /// Creates a new set with the specified initial capacity and load factor. 
        /// This set will hold initialCapacity items before growing the backing table
        /// </summary>
        /// <param name="initialCapacity">If not a power of two, it is increased to the next nearest power of two</param>
        /// <param name="loadFactor">the initial load factor</param>
        public ObjectSet(int initialCapacity = 51, float loadFactor = 0.8f)
        {
            if (initialCapacity < 0)
            {
                throw new ArgumentException("initialCapacity must be >= 0: " + initialCapacity);
            }

            initialCapacity = EssentialMathUtils.NextPowerOfTwo((int)Math.Ceiling(initialCapacity / loadFactor));
            if (initialCapacity > 1 << 30)
            {
                throw new ArgumentException("initialCapacity is too large: " + initialCapacity);
            }

            this.capacity = initialCapacity;

            if (loadFactor <= 0)
            {
                throw new ArgumentException("loadFactor must be > 0: " + loadFactor);
            }

            this.loadFactor = loadFactor;

            this.threshold = (int)(this.capacity * loadFactor);
            this.mask = this.capacity - 1;
            this.hashShift = 31 - EssentialMathUtils.NumberOfTrailingZeros(this.capacity);
            this.stashCapacity = Math.Max(3, (int)Math.Ceiling(Math.Log(this.capacity)) * 2);
            this.pushIterations = Math.Max(Math.Min(this.capacity, 8), (int)Math.Sqrt(this.capacity) / 8);

            this.keyTable = new T[this.capacity + this.stashCapacity];
        }

        /// <summary>
        /// Creates a new set identical to the specified set
        /// </summary>
        /// <param name="other">the set to copy from</param>
        public ObjectSet(ObjectSet<T> other)
            : this((int)Math.Floor(other.capacity * other.loadFactor), other.loadFactor)
        {
            this.stashSize = other.stashSize;
            Array.Copy(other.keyTable, 0, this.keyTable, 0, other.keyTable.Length);
            this.Size = other.Size;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public int Size { get; internal set; }

        /// <summary>
        /// If this set already contains the key, the call leaves the set unchanged and returns false
        /// </summary>
        /// <param name="key">the key to add</param>
        /// <returns>true if the key was not already in the set</returns>
        public bool Add(T key)
        {
            if (key == null)
            {
                throw new ArgumentException("key cannot be null.");
            }

            T[] table = this.keyTable;

            // Check for existing keys.
            int hashCode = key.GetHashCode();
            int index1 = hashCode & this.mask;
            T key1 = table[index1];
            if (key.Equals(key1))
            {
                return false;
            }

            int index2 = this.Hash2(hashCode);
            T key2 = table[index2];
            if (key.Equals(key2))
            {
                return false;
            }

            int index3 = this.Hash3(hashCode);
            T key3 = table[index3];
            if (key.Equals(key3))
            {
                return false;
            }

            // Find key in the stash.
            for (int i = this.capacity, n = i + this.stashSize; i < n; i++)
            {
                if (key.Equals(table[i]))
                {
                    return false;
                }
            }

            // Check for empty buckets.
            if (key1 == null)
            {
                table[index1] = key;
                if (this.Size++ >= this.threshold)
                {
                    this.Resize(this.capacity << 1);
                }

                return true;
            }

            if (key2 == null)
            {
                table[index2] = key;
                if (this.Size++ >= this.threshold)
                {
                    this.Resize(this.capacity << 1);
                }

                return true;
            }

            if (key3 == null)
            {
                table[index3] = key;
                if (this.Size++ >= this.threshold)
                {
                    this.Resize(this.capacity << 1);
                }

                return true;
            }

            this.Push(key, index1, key1, index2, key2, index3, key3);
            return true;
        }

        public void AddAll(T[] array)
        {
            this.AddAll(array, 0, array.Length);
        }
        
        public void AddAll(T[] array, int offset, int length)
        {
            this.EnsureCapacity(length);
            for (int i = offset, n = i + length; i < n; i++)
            {
                this.Add(array[i]);
            }
        }

        public void AddAll(ObjectSet<T> set)
        {
            this.EnsureCapacity(set.Size);
            foreach (T key in set)
            {
                this.Add(key);
            }
        }

        /// <summary>
        /// Skips checks for existing keys
        /// </summary>
        /// <param name="key">the key to add</param>
        public void AddResize(T key)
        {
            // Check for empty buckets.
            int hashCode = key.GetHashCode();
            int index1 = hashCode & this.mask;
            T key1 = this.keyTable[index1];
            if (key1 == null)
            {
                this.keyTable[index1] = key;
                if (this.Size++ >= this.threshold)
                {
                    this.Resize(this.capacity << 1);
                }

                return;
            }

            int index2 = this.Hash2(hashCode);
            T key2 = this.keyTable[index2];
            if (key2 == null)
            {
                this.keyTable[index2] = key;
                if (this.Size++ >= this.threshold)
                {
                    this.Resize(this.capacity << 1);
                }

                return;
            }

            int index3 = this.Hash3(hashCode);
            T key3 = this.keyTable[index3];
            if (key3 == null)
            {
                this.keyTable[index3] = key;
                if (this.Size++ >= this.threshold)
                {
                    this.Resize(this.capacity << 1);
                }
                
                return;
            }

            this.Push(key, index1, key1, index2, key2, index3, key3);
        }

        public void AddStash(T key)
        {
            if (this.stashSize == this.stashCapacity)
            {
                // Too many pushes occurred and the stash is full, increase the table size.
                this.Resize(this.capacity << 1);
                this.Add(key);
                return;
            }

            // Store key in the stash.
            int index = this.capacity + this.stashSize;
            this.keyTable[index] = key;
            this.stashSize++;
            this.Size++;
        }

        public void Push(T insertKey, int index1, T key1, int index2, T key2, int index3, T key3)
        {
            T[] table = this.keyTable;
            int currentMask = this.mask;

            // Push keys until an empty bucket is found.
            T evictedKey;
            int i = 0, iterations = this.pushIterations;
            do
            {
                // Replace the key and value for one of the hashes.
                switch (UnityEngine.Random.Range(0, 2))
                {
                    case 0:
                        evictedKey = key1;
                        table[index1] = insertKey;
                        break;
                    case 1:
                        evictedKey = key2;
                        table[index2] = insertKey;
                        break;
                    default:
                        evictedKey = key3;
                        table[index3] = insertKey;
                        break;
                }

                // If the evicted key hashes to an empty bucket, put it there and stop.
                int hashCode = evictedKey.GetHashCode();
                index1 = hashCode & currentMask;
                key1 = table[index1];
                if (key1 == null)
                {
                    table[index1] = evictedKey;
                    if (this.Size++ >= this.threshold)
                    {
                        this.Resize(this.capacity << 1);
                    }

                    return;
                }

                index2 = this.Hash2(hashCode);
                key2 = table[index2];
                if (key2 == null)
                {
                    table[index2] = evictedKey;
                    if (this.Size++ >= this.threshold)
                    {
                        this.Resize(this.capacity << 1);
                    }

                    return;
                }

                index3 = this.Hash3(hashCode);
                key3 = table[index3];
                if (key3 == null)
                {
                    table[index3] = evictedKey;
                    if (this.Size++ >= this.threshold)
                    {
                        this.Resize(this.capacity << 1);
                    }

                    return;
                }

                if (++i == iterations)
                {
                    break;
                }

                insertKey = evictedKey;
            }
            while (true);

            this.AddStash(evictedKey);
        }

        public bool Remove(T key)
        {
            int hashCode = key.GetHashCode();
            int index = hashCode & this.mask;
            if (key.Equals(this.keyTable[index]))
            {
                this.keyTable[index] = default(T);
                this.Size--;
                return true;
            }

            index = this.Hash2(hashCode);
            if (key.Equals(this.keyTable[index]))
            {
                this.keyTable[index] = default(T);
                this.Size--;
                return true;
            }

            index = this.Hash3(hashCode);
            if (key.Equals(this.keyTable[index]))
            {
                this.keyTable[index] = default(T);
                this.Size--;
                return true;
            }

            return this.RemoveStash(key);
        }

        public bool RemoveStash(T key)
        {
            T[] table = this.keyTable;
            for (int i = this.capacity, n = i + this.stashSize; i < n; i++)
            {
                if (key.Equals(table[i]))
                {
                    this.RemoveStashIndex(i);
                    this.Size--;
                    return true;
                }
            }

            return false;
        }

        public void RemoveStashIndex(int index)
        {
            // If the removed location was not last, move the last tuple to the removed location.
            this.stashSize--;
            int lastIndex = this.capacity + this.stashSize;
            if (index < lastIndex)
            {
                this.keyTable[index] = this.keyTable[lastIndex];
            }
        }

        public void Shrink(int maxCapacity)
        {
            if (maxCapacity < 0)
            {
                throw new ArgumentException("maximumCapacity must be >= 0: " + maxCapacity);
            }

            if (this.Size > maxCapacity)
            {
                maxCapacity = this.Size;
            }

            if (this.capacity <= maxCapacity)
            {
                return;
            }

            maxCapacity = EssentialMathUtils.NextPowerOfTwo(maxCapacity);
            this.Resize(maxCapacity);
        }

        public void Clear(int maxCapacity)
        {
            if (this.capacity <= maxCapacity)
            {
                this.Clear();
                return;
            }

            this.Size = 0;
            this.Resize(maxCapacity);
        }

        public void Clear()
        {
            if (this.Size == 0)
            {
                return;
            }

            T[] table = this.keyTable;
            for (int i = this.capacity + this.stashSize; i-- > 0;)
            {
                table[i] = default(T);
            }

            this.Size = 0;
            this.stashSize = 0;
        }

        public bool Contains(T key)
        {
            int hashCode = key.GetHashCode();
            int index = hashCode & this.mask;
            if (!key.Equals(this.keyTable[index]))
            {
                index = this.Hash2(hashCode);
                if (!key.Equals(this.keyTable[index]))
                {
                    index = this.Hash3(hashCode);
                    if (!key.Equals(this.keyTable[index]))
                    {
                        return this.ContainsKeyStash(key);
                    }
                }
            }

            return true;
        }

        public bool ContainsKeyStash(T key)
        {
            T[] table = this.keyTable;
            for (int i = this.capacity, n = i + this.stashSize; i < n; i++)
            {
                if (key.Equals(table[i]))
                {
                    return true;
                }
            }

            return false;
        }

        public T First()
        {
            T[] table = this.keyTable;
            for (int i = 0, n = this.capacity + this.stashSize; i < n; i++)
            {
                if (table[i] != null)
                {
                    return table[i];
                }
            }

            throw new IllegalStateException("ObjectSet is empty.");
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new ObjectSetIterator<T>(this);
        }

        public override int GetHashCode()
        {
            int h = 0;
            for (int i = 0, n = this.capacity + this.stashSize; i < n; i++)
            {
                if (this.keyTable[i] != null)
                {
                    h += this.keyTable[i].GetHashCode();
                }
            }

            return h;
        }

        public override bool Equals(object obj)
        {
            ObjectSet<T> other = obj as ObjectSet<T>;
            if (other == null || other.Size != this.Size)
            {
                return false;
            }

            for (int i = 0, n = this.capacity + this.stashSize; i < n; i++)
            {
                if (this.keyTable[i] != null && !other.Contains(this.keyTable[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public override string ToString()
        {
            return string.Format("{{{0}}}", this.ToString(", "));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void EnsureCapacity(int additionalCapacity)
        {
            int sizeNeeded = this.Size + additionalCapacity;
            if (sizeNeeded >= this.threshold)
            {
                this.Resize(EssentialMathUtils.NextPowerOfTwo((int)Math.Ceiling(sizeNeeded / this.loadFactor)));
            }
        }

        private void Resize(int newSize)
        {
            int oldEndIndex = this.capacity + this.stashSize;

            this.capacity = newSize;
            this.threshold = (int)(newSize * this.loadFactor);
            this.mask = newSize - 1;
            this.hashShift = 31 - EssentialMathUtils.NumberOfTrailingZeros(newSize);
            this.stashCapacity = Math.Max(3, (int)Math.Ceiling(Math.Log(newSize)) * 2);
            this.pushIterations = Math.Max(Math.Min(newSize, 8), (int)Math.Sqrt(newSize) / 8);

            T[] oldKeyTable = this.keyTable;

            this.keyTable = new T[newSize + this.stashCapacity];

            int oldSize = this.Size;
            this.Size = 0;
            this.stashSize = 0;
            if (oldSize > 0)
            {
                for (int i = 0; i < oldEndIndex; i++)
                {
                    T key = oldKeyTable[i];
                    if (key != null)
                    {
                        this.AddResize(key);
                    }
                }
            }
        }

        private int Hash2(int h)
        {
            h *= Prime2;
            return (h ^ h >> this.hashShift) & this.mask;
        }

        private int Hash3(int h)
        {
            h *= Prime3;
            return (h ^ h >> this.hashShift) & this.mask;
        }

        private string ToString(string separator)
        {
            if (this.Size == 0)
            {
                return string.Empty;
            }

            StringBuilder buffer = new StringBuilder(32);
            T[] table = this.keyTable;
            int i = table.Length;
            while (i-- > 0)
            {
                T key = table[i];
                if (key == null)
                {
                    continue;
                }

                buffer.Append(key);
                break;
            }

            while (i-- > 0)
            {
                T key = table[i];
                if (key == null)
                {
                    continue;
                }

                buffer.Append(separator);
                buffer.Append(key);
            }

            return buffer.ToString();
        }
    }
}
