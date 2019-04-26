namespace Craiel.UnityEssentials.Runtime.Data.SBT.Nodes
{
    using System;
    using System.IO;
    using Enums;
    using SBT;

    public abstract class SBTNodeArray<T> : ISBTNode
    {
        private const uint CapacityInterval = 100;

        // Default to 10 MB for simple type arrays
        private const int SimpleTypeCapacityLimit = 1024 * 1024 * 10;
        private const int DefaultEntryCountLimit = 1000000;
        
        private readonly byte dataEntrySize;
        private readonly SBTType baseType;
        
        private int capacityLimit;

        private T[] data;
        private int nextDataIndex;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        protected SBTNodeArray(SBTType type, SBTFlags flags, string note)
        {
            this.Type = type;
            this.Flags = flags;
            this.Note = note;
            this.data = new T[0];
            
            this.baseType = type.GetArrayBaseType();

            byte? dataEntrySize = SBTUtils.GetDataEntrySize(this.baseType);
            if (dataEntrySize == null)
            {
                this.capacityLimit = DefaultEntryCountLimit;
            }
            else
            {
                this.capacityLimit = SimpleTypeCapacityLimit / dataEntrySize.Value;
            }
        }
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public SBTType Type { get; }
        
        public SBTFlags Flags { get; }
        
        public string Note { get; }

        public int Capacity
        {
            get { return this.capacityLimit; }
        }

        public int Length
        {
            get { return this.nextDataIndex; }
        }

        public void SetCapacityLimit(int newLimit)
        {
            if (newLimit <= 0)
            {
                throw new ArgumentException("Invalid Capacity limit value", nameof(newLimit));
            }

            this.capacityLimit = newLimit;
        }

        public void SetCapacity(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentException("Invalid Capacity Value", nameof(capacity));
            }
            
            this.CheckCapacityLimit(capacity);
            
            Array.Resize(ref this.data, capacity);
            
            if (this.Length > this.data.Length)
            {
                // Clamp the next free slot index
                this.nextDataIndex = this.data.Length;
            }
        }

        public void AddChecked(T entry)
        {
            if (this.Length >= this.data.Length)
            {
                this.SetCapacity((ushort) (this.data.Length + CapacityInterval));
            }

            this.Add(entry);
        }
        
        public void Add(T entry)
        {
            this.data[this.nextDataIndex++] = entry;
        }
        
        public void AddChecked(T[] entries)
        {
            for (var i = 0; i < entries.Length; i++)
            {
                this.AddChecked(entries[i]);
            }
        }
        
        public void Add(T[] entries)
        {
            for (var i = 0; i < entries.Length; i++)
            {
                this.data[this.nextDataIndex++] = entries[i];
            }
        }

        public void Set(int index, T entry)
        {
            this.data[index] = entry;
        }
        
        public T Read(int index)
        {
            return this.data[index];
        }

        public T TryRead(int index)
        {
            if (index < 0 || this.data.Length <= index)
            {
                return default;
            }

            return this.Read(index);
        }
        
        public void Save(BinaryWriter writer)
        {
            int count = this.Length;
            
            writer.Write(this.capacityLimit);
            writer.Write(count);
            for (var i = 0; i < this.nextDataIndex; i++)
            {
                this.SerializeOne(writer, this.data[i]);
            }
        }

        public void Load(BinaryReader reader)
        {
            this.SetCapacityLimit(reader.ReadInt32());
            this.nextDataIndex = reader.ReadInt32();
            this.SetCapacity(this.nextDataIndex);
            for (var i = 0; i < this.nextDataIndex; i++)
            {
                this.data[i] = this.DeserializeOne(reader);
            }
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected abstract void SerializeOne(BinaryWriter writer, T entry);
        protected abstract T DeserializeOne(BinaryReader reader);
        
        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void CheckCapacityLimit(int newLimit)
        {
            if (newLimit <= 0 || newLimit > this.capacityLimit)
            {
                throw new ArgumentOutOfRangeException(nameof(newLimit));
            }
        }
    }
}