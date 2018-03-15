namespace Craiel.UnityEssentials.Collections
{
    using System;

    public class CircularBuffer<T>
    {
        private T[] items;

        private int head;
        private int tail;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------

        /// <summary>
        /// Creates a <see cref="CircularBuffer{T}"/>
        /// </summary>
        /// <param name="initialCapacity">the initial capacity of this circular buffer</param>
        /// <param name="resizable">whether this buffer is resizable or has fixed capacity</param>
        public CircularBuffer(int initialCapacity = 16, bool resizable = true)
        {
            this.items = new T[initialCapacity];
            this.Resizable = resizable;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool Resizable { get; set; }

        public bool IsEmpty
        {
            get
            {
                return this.Size == 0;
            }
        }

        public bool IsFull
        {
            get
            {
                return this.Size == this.items.Length;
            }
        }

        public int Size { get; private set; }

        /// <summary>
        /// Adds the given item to the tail of this circular buffer
        /// </summary>
        /// <param name="item">the item to add</param>
        /// <returns> <code>true</code> if the item has been successfully added to this circular buffer; <code>false</code> otherwise</returns>
        public bool Store(T item)
        {
            if (this.Size == this.items.Length)
            {
                if (!this.Resizable)
                {
                    return false;
                }

                this.Resize((int)Math.Max(8, this.items.Length * 1.75f));
            }

            this.Size++;
            this.items[this.tail++] = item;
            if (this.tail == this.items.Length)
            {
                this.tail = 0;
            }

            return true;
        }

        /// <summary>
        /// Removes and returns the item at the head of this circular buffer (if any).
        /// </summary>
        /// <returns>the item just removed or <code>default(T)</code> if this circular buffer is empty</returns>
        public T Read()
        {
            if (this.Size > 0)
            {
                this.Size--;
                T item = this.items[this.head];

                // Avoid keeping useless references
                this.items[this.head] = default(T);

                if (++this.head == this.items.Length)
                {
                    this.head = 0;
                }

                return item;
            }

            return default(T);
        }

        /// <summary>
        /// Removes all items from this circular buffer
        /// </summary>
        public void Clear()
        {
            if (this.tail > this.head)
            {
                int i = this.head;
                int n = this.tail;
                do
                {
                    this.items[i++] = default(T);
                }
                while (i < n);
            }
            else if (this.Size > 0)
            {
                // NOTE: when head == tail the buffer can be empty or full
                int i = this.head;
                int n = this.items.Length;
                do
                {
                    this.items[i++] = default(T);
                }
                while (i < n);

                i = 0;
                n = this.tail;
                do
                {
                    this.items[i++] = default(T);
                }
                while (i < n);
            }

            this.head = 0;
            this.tail = 0;
            this.Size = 0;
        }

        /// <summary>
        /// Increases the size of the backing array (if necessary) to accommodate the specified number of additional items. 
        /// Useful before adding many items to avoid multiple backing array resizes
        /// </summary>
        /// <param name="additionalCapacity">the number of additional items</param>
        public void EnsureCapacity(int additionalCapacity)
        {
            int newCapacity = this.Size + additionalCapacity;
            if (this.items.Length < newCapacity)
            {
                this.Resize(newCapacity);
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------

        /// <summary>
        /// Creates a new backing array with the specified capacity containing the current items
        /// </summary>
        /// <param name="newCapacity">the new capacity</param>
        private void Resize(int newCapacity)
        {
            T[] newItems = new T[newCapacity];
            if (this.tail > this.head)
            {
                Array.Copy(this.items, this.head, newItems, 0, this.Size);
            }
            else
            {
                Array.Copy(this.items, this.head, newItems, 0, this.items.Length - this.head);
                Array.Copy(this.items, 0, newItems, this.items.Length - this.head, this.tail);
            }

            this.head = 0;
            this.tail = this.Size;
            this.items = newItems;
        }
    }
}
