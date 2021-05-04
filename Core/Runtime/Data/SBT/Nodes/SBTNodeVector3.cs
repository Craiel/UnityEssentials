namespace Craiel.UnityEssentials.Runtime.Data.SBT.Nodes
{
    using System;
    using System.Globalization;
    using System.IO;
    using Enums;
    using Extensions;
    using UnityEngine;

    public class SBTNodeVector3 : ISBTNode
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SBTNodeVector3(Vector3 data, SBTFlags flags = SBTFlags.None, string note = null)
        {
            this.Data = data;
            this.Flags = flags;
            this.Note = note;
        }
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public readonly Vector3 Data;
        
        public SBTFlags Flags { get; }
        
        public string Note { get; }

        public SBTType Type
        {
            get { return SBTType.Vector3; }
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