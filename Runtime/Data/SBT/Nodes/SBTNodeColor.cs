namespace Craiel.UnityEssentials.Runtime.Data.SBT.Nodes
{
    using System;
    using System.IO;
    using Enums;
    using Extensions;
    using UnityEngine;

    public class SBTNodeColor : ISBTNode
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SBTNodeColor(Color data, SBTFlags flags = SBTFlags.None)
        {
            this.Data = data;
            this.Flags = flags;
        }
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public readonly Color Data;
        
        public SBTFlags Flags { get; }

        public SBTType Type
        {
            get { return SBTType.Color; }
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