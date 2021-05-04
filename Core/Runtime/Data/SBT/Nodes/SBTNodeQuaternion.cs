namespace Craiel.UnityEssentials.Runtime.Data.SBT.Nodes
{
    using System;
    using System.Globalization;
    using System.IO;
    using Enums;
    using Extensions;
    using UnityEngine;

    public class SBTNodeQuaternion : ISBTNode
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SBTNodeQuaternion(Quaternion data, SBTFlags flags = SBTFlags.None, string note = null)
        {
            this.Data = data;
            this.Flags = flags;
            this.Note = note;
        }
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public readonly Quaternion Data;
        
        public SBTFlags Flags { get; }
        
        public string Note { get; }

        public SBTType Type
        {
            get { return SBTType.Quaternion; }
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