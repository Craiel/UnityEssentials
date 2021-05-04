namespace Craiel.UnityEssentials.Runtime.Data.SBT.Nodes
{
    using System;
    using System.IO;
    using Enums;

    public struct SBTNodeLong : ISBTNode
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SBTNodeLong(long data, SBTFlags flags = SBTFlags.None, string note = null)
        {
            this.Data = data;
            this.Flags = flags;
            this.Note = note;
        }
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public readonly long Data;
        
        public SBTFlags Flags { get; }
        
        public string Note { get; }

        public SBTType Type
        {
            get { return SBTType.Long; }
        }
        
        public void Save(BinaryWriter writer)
        {
            writer.Write(this.Data);
        }
        
        public void Load(BinaryReader reader)
        {
            throw new InvalidOperationException();
        }
    }
}