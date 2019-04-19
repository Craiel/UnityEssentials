namespace Craiel.UnityEssentials.Runtime.Data.SBT.Nodes
{
    using System;
    using System.IO;
    using Enums;
    using Extensions;
    using UnityEngine;

    public class SBTNodeVector2 : ISBTNode
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SBTNodeVector2(Vector2 data, SBTFlags flags = SBTFlags.None)
        {
            this.Data = data;
            this.Flags = flags;
        }
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public readonly Vector2 Data;
        
        public SBTFlags Flags { get; }

        public SBTType Type
        {
            get { return SBTType.Vector2; }
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