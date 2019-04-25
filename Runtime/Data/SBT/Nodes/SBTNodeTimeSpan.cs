namespace Craiel.UnityEssentials.Runtime.Data.SBT.Nodes
{
    using System;
    using System.IO;
    using Enums;
    using Extensions;

    public class SBTNodeTimeSpan : ISBTNode
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SBTNodeTimeSpan(TimeSpan data, SBTFlags flags = SBTFlags.None)
        {
            this.Data = data;
            this.Flags = flags;
        }
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public readonly TimeSpan Data;
        
        public SBTFlags Flags { get; }

        public SBTType Type
        {
            get { return SBTType.TimeSpan; }
        }
        
        public void Serialize(BinaryWriter writer)
        {
            writer.Write(this.Data);
        }
        
        public void Deserialize(BinaryReader reader)
        {
            throw new InvalidOperationException();
        }
    }
}