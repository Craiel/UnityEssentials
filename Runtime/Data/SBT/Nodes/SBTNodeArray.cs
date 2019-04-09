namespace Craiel.UnityEssentials.Runtime.Data.SBT.Nodes
{
    using System;
    using System.IO;
    using Enums;
    using SBT;

    public class SBTNodeArray : ISBTNode
    {
        private const int CapacityInterval = 10;
        
        private readonly SBTType baseType;

        private object[] data;
        private int dataIndex;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SBTNodeArray(SBTType type, SBTFlags flags)
        {
            this.Type = type;
            this.Flags = flags;
            this.data = new object[0];
            
            this.baseType = type.GetArrayBaseType();
        }
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public SBTFlags Flags { get; }

        public SBTType Type { get; }

        public int Length
        {
            get { return this.dataIndex + 1; }
        }

        public void SetCapacity(int capacity)
        {
            Array.Resize(ref this.data, capacity);
            
            if (this.Length > this.data.Length)
            {
                // Clamp the next free slot index
                this.dataIndex = this.data.Length - 1;
            }
        }

        public void AddEntry(object entry)
        {
            if (this.Length >= this.data.Length)
            {
                this.SetCapacity(this.data.Length + CapacityInterval);
            }

            this.data[++this.dataIndex] = entry;
        }
        
        public object Read(int index)
        {
            return this.data[index];
        }

        public object TryRead(int index)
        {
            if (index < 0 || this.data.Length <= index)
            {
                return null;
            }

            return this.Read(index);
        }

        public void Serialize(BinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void Deserialize(BinaryReader reader)
        {
            throw new System.NotImplementedException();
        }
    }
}