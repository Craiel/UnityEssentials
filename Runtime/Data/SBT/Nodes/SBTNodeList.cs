namespace Craiel.UnityEssentials.Runtime.Data.SBT.Nodes
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Enums;
    using SBT;
    using UnityEngine;

    public class SBTNodeList : ISBTNode
    {
        private readonly IList<ISBTNode> children;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SBTNodeList(SBTFlags flags = SBTFlags.None)
        {
            this.children = new List<ISBTNode>();
            
            this.Flags = flags;
        }
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public int Count
        {
            get { return this.children.Count; }
        }
        
        public SBTFlags Flags { get; set; }

        public SBTType Type
        {
            get { return SBTType.List; }
        }
        
        public ISBTNode AddEntry(SBTType type, object data = null, SBTFlags flags = SBTFlags.None)
        {
            var node = SBTUtils.GetNode(type, data, flags);
            this.AddEntry(node);
            return node;
        }
        
        public void AddEntry<T>(T child)
            where T : ISBTNode
        {
            if (this.children.Count >= ushort.MaxValue)
            {
                throw new InvalidDataException("Node List limit exceeded!");
            }
            
            this.children.Add(child);
        }

        public T Read<T>(int index)
            where T : ISBTNode
        {
            return (T) this.children[index];
        }
        
        public ISBTNode Read(int index)
        {
            return this.children[index];
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write((ushort)this.children.Count);
            for (var i = 0; i < this.children.Count; i++)
            {
                ISBTNode child = this.children[i];
                child.WriteHeader(writer);
                child.Serialize(writer);
            }
        }
        
        public void Deserialize(BinaryReader reader)
        {
            ushort count = reader.ReadUInt16();
            for (var i = 0; i < count; i++)
            {
                SBTType type;
                SBTFlags flags;
                SBTUtils.ReadHeader(reader, out type, out flags);

                object data = null;
                if (type.IsSimpleType())
                {
                    data = SBTUtils.ReadSimpleTypeData(type, reader);
                }
                
                ISBTNode child = SBTUtils.GetNode(type, data, flags);
                if (!type.IsSimpleType())
                {
                    child.Deserialize(reader);
                }
                
                this.AddEntry(child);
            }
        }
    }
}