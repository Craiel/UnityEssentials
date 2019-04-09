namespace Craiel.UnityEssentials.Runtime.Data.SBT.Nodes
{
    using System.Collections.Generic;
    using System.IO;
    using Enums;
    using SBT;

    public class SBTNodeDictionary : ISBTNode
    {
        private readonly IDictionary<string, ISBTNode> children;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SBTNodeDictionary(SBTFlags flags = SBTFlags.None)
        {
            this.children = new Dictionary<string, ISBTNode>();
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
            get { return SBTType.Dictionary; }
        }
        
        public ISBTNode AddEntry(string key, SBTType type, object data = null, SBTFlags flags = SBTFlags.None)
        {
            var node = SBTUtils.GetNode(type, data, flags);
            this.AddEntry(key, node);
            return node;
        }
        
        public void AddEntry<T>(string key, T child) 
            where T : ISBTNode
        {
            this.children.Add(key, child);
        }
        
        public T Read<T>(string key)
            where T : ISBTNode
        {
            return (T) this.children[key];
        }
        
        public ISBTNode Read(string key)
        {
            return this.children[key];
        }
        
        public T TryRead<T>(string key)
            where T : ISBTNode
        {
            if (this.children.TryGetValue(key, out ISBTNode result))
            {
                return (T) result;
            }

            return default;
        }

        public bool Contains(string key)
        {
            return this.children.ContainsKey(key);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write((ushort)this.children.Count);
            foreach (KeyValuePair<string,ISBTNode> entry in this.children)
            {
                writer.Write(entry.Key);
                entry.Value.WriteHeader(writer);
                entry.Value.Serialize(writer);
            }
        }

        public void Deserialize(BinaryReader reader)
        {
            int count = reader.ReadUInt16();
            for (var i = 0; i < count; i++)
            {
                string key = reader.ReadString();
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
                
                this.AddEntry(key, child);
            }
        }
    }
}