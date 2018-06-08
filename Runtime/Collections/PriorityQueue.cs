namespace Craiel.UnityEssentials.Runtime.Collections
{
    using System;

    /// <summary>
    /// An unbounded priority queue based on a priority heap. The elements of the priority queue are ordered according to their
    /// <see cref="IComparable"/>. A priority queue does not permit <code>null</code> elements.
    /// <para>
    /// The queue can be set to accept or reject the insertion of non unique elements through the method {@code setUniqueness}.
    /// Uniqueness is disabled by default.
    /// </para>
    /// <para>
    /// The <em>head</em> of this queue is the <em>least</em> element with respect to the specified ordering. 
    /// If multiple elements are tied for least value(provided that uniqueness is set to false), 
    /// the head is one of those elements -- ties are broken arbitrarily.
    /// The queue retrieval operations { @code poll}, {@code remove}, {@code peek}, and {@code element} access the element
    /// at the head of the queue.
    /// </para>
    /// <para>
    /// A priority queue is unbounded, but has an internal <i>capacity</i> governing the size of an array used to store the elements on the queue.
    /// It is always at least as large as the queue size.As elements are added to a priority queue, its capacity grows automatically.
    /// </para>
    /// <para>
    /// Iterating the queue with the method {@link #get(int) get} is <em>not</em> guaranteed to traverse the elements of the priority queue in any particular order.
    /// </para>
    /// <para>
    /// Implementation note: this implementation provides O(log(n)) time for the enqueuing and dequeing methods ({@code add} and
    /// * {@code poll}; and constant time for the retrieval methods({ @code peek} and {@code size}).
    /// </para>
    /// </summary>
    /// <typeparam name="T">the type of comparable elements held in this queue</typeparam>
    public class PriorityQueue<T>
        where T : IComparable
    {
        private const int DefaultInitialCapacity = 11;

        private const float CapacityRatioLow = 1.5f;

        private const float CapacityRatioHigh = 2f;

        /// <summary>
        /// A set used to check elements uniqueness (if enabled)
        /// </summary>
        private readonly ObjectSet<T> set;

        /// <summary>
        /// Priority queue represented as a balanced binary heap: the two children of queue[n] are queue[2*n+1] and queue[2*(n+1)]. The
        /// priority queue is ordered by the elements' natural ordering: For each node n in the heap and each descendant d of n, n &lt;= d.
        /// The element with the lowest value is in queue[0], assuming the queue is nonempty
        /// </summary>
        private T[] queue;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------

        /// <summary>
        /// Creates a <see cref="PriorityQueue{T}"/> with the specified initial capacity that orders its elements according to their
        /// <see cref="IComparable"/>
        /// </summary>
        /// <param name="initialCapacity"></param>
        public PriorityQueue(int initialCapacity = DefaultInitialCapacity)
        {
            this.queue = new T[initialCapacity];
            this.set = new ObjectSet<T>(initialCapacity);
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------

        /// <summary>
        /// A flag indicating whether elements inserted into the queue must be unique
        /// </summary>
        public bool Uniqueness { get; set; }

        /// <summary>
        /// The number of elements in the priority queue
        /// </summary>
        public int Size { get; private set; }

        /// <summary>
        /// Inserts the specified element into this priority queue. If <see cref="Uniqueness"/> is enabled and this priority queue already
        /// contains the element, the call leaves the queue unchanged and returns false
        /// </summary>
        /// <param name="entry">the entry to add</param>
        /// <returns>true if the element was added to this queue, else false</returns>
        public bool Add(T entry)
        {
            if (entry == null)
            {
                throw new ArgumentException("Element cannot be null.");
            }

            if (this.Uniqueness && !this.set.Add(entry))
            {
                return false;
            }

            int i = this.Size;
            if (i >= this.queue.Length)
            {
                this.GrowToSize(i + 1);
            }

            this.Size = i + 1;
            if (i == 0)
            {
                this.queue[0] = entry;
            }
            else
            {
                this.SiftUp(i, entry);
            }

            return true;
        }

        /// <summary>
        /// Retrieves, but does not remove, the head of this queue. If this queue is empty <code>null</code> is returned.
        /// </summary>
        /// <returns>the head of this queue</returns>
        public T Peek()
        {
            return this.Size == 0 ? default(T) : this.queue[0];
        }

        /// <summary>
        /// Retrieves the element at the specified index. If such an element doesn't exist <code>null</code> is returned.
        /// Iterating the queue by index is <em>not</em> guaranteed to traverse the elements in any particular order.
        /// </summary>
        /// <param name="index">the index of the desired element</param>
        /// <returns>the element at the specified index in this queue</returns>
        public T Get(int index)
        {
            return index >= this.Size ? default(T) : this.queue[index];
        }

        /// <summary>
        /// Removes all of the elements from this priority queue. The queue will be empty after this call returns
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < this.Size; i++)
            {
                this.queue[i] = default(T);
            }

            this.Size = 0;
            this.set.Clear();
        }

        /// <summary>
        /// Retrieves and removes the head of this queue, or returns <code>null</code> if this queue is empty
        /// </summary>
        /// <returns>the head of this queue, or <code>null</code> if this queue is empty</returns>
        public T Poll()
        {
            if (this.Size == 0)
            {
                return default(T);
            }

            int s = --this.Size;
            T result = this.queue[0];
            T x = this.queue[s];
            this.queue[s] = default(T);
            if (s != 0)
            {
                this.SiftDown(0, x);
            }

            if (this.Uniqueness)
            {
                this.set.Remove(result);
            }

            return result;
        }

        /// <summary>
        /// Inserts item at position pos, maintaining heap invariant by promoting x up the tree until it is greater than or equal to its
        /// parent, or is the root.
        /// </summary>
        /// <param name="pos">the position to fill</param>
        /// <param name="itemToAdd">the item to insert</param>
        public void SiftUp(int pos, T itemToAdd)
        {
            while (pos > 0)
            {
                int parent = (pos - 1) >> 1;
                T e = this.queue[parent];
                if (itemToAdd.CompareTo(e) >= 0)
                {
                    break;
                }

                this.queue[pos] = e;
                pos = parent;
            }

            this.queue[pos] = itemToAdd;
        }

        /// <summary>
        /// Inserts item at position pos, maintaining heap invariant by demoting x down the tree repeatedly until it is less than or
        /// equal to its children or is a leaf.
        /// </summary>
        /// <param name="pos">the position to fill</param>
        /// <param name="itemToInsert">the item to insert</param>
        public void SiftDown(int pos, T itemToInsert)
        {
            int half = this.Size >> 1; // loop while a non-leaf
            while (pos < half)
            {
                int child = (pos << 1) + 1; // assume left child is least
                T c = this.queue[child];
                int right = child + 1;
                if (right < this.Size && c.CompareTo(this.queue[right]) > 0)
                {
                    c = this.queue[child = right];
                }

                if (itemToInsert.CompareTo(c) <= 0)
                {
                    break;
                }

                this.queue[pos] = c;
                pos = child;
            }

            this.queue[pos] = itemToInsert;
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------

        /// <summary>
        /// Increases the capacity of the array
        /// </summary>
        /// <param name="minCapacity">the desired minimum capacity</param>
        private void GrowToSize(int minCapacity)
        {
            // overflow
            if (minCapacity < 0)
            {
                throw new InvalidOperationException("Capacity upper limit exceeded.");
            }

            int oldCapacity = this.queue.Length;

            // Double size if small; else grow by 50%
            int newCapacity = (int)(oldCapacity < 64 ? (oldCapacity + 1) * CapacityRatioHigh : oldCapacity * CapacityRatioLow);

            // overflow
            if (newCapacity < 0)
            {
                newCapacity = int.MaxValue;
            }

            if (newCapacity < minCapacity)
            {
                newCapacity = minCapacity;
            }

            T[] newQueue = new T[newCapacity];
            Array.Copy(this.queue, 0, newQueue, 0, this.Size);
            this.queue = newQueue;
        }
    }
}
