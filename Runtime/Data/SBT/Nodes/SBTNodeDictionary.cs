namespace Craiel.UnityEssentials.Runtime.Data.SBT.Nodes
{
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using Enums;
    using SBT;

    public class SBTNodeDictionary : ISBTNode, IEnumerable<string>
    {
        private readonly IDictionary<string, ISBTNode> children;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SBTNodeDictionary(SBTFlags flags = SBTFlags.None, string note = null)
        {
            this.children = new Dictionary<string, ISBTNode>();
            this.Flags = flags;
            this.Note = note;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public int Count
        {
            get { return this.children.Count; }
        }
        
        public SBTFlags Flags { get; set; }
        
        public string Note { get; }

        public SBTType Type
        {
            get { return SBTType.Dictionary; }
        }
        
        public ISBTNode AddEntry(string key, SBTType type, object data = null, SBTFlags flags = SBTFlags.None, string note = null)
        {
            var node = SBTUtils.GetNode(type, data, flags, note);
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
        
        public bool TryRead<T>(string key, out T result)
            where T : ISBTNode
        {
            result = default;
            if (!this.children.TryGetValue(key, out ISBTNode node))
            {
                return false;
            }

            if (node is T)
            {
                result = (T) node;
                return true;
            }

            return false;
        }

        public bool Contains(string key)
        {
            return this.children.ContainsKey(key);
        }
        
        public void Save(BinaryWriter writer)
        {
            writer.Write((ushort)this.children.Count);
            foreach (KeyValuePair<string,ISBTNode> entry in this.children)
            {
                writer.Write(entry.Key);
                entry.Value.WriteHeader(writer);
                entry.Value.Save(writer);
            }
        }

        public void Load(BinaryReader reader)
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
                    child.Load(reader);
                }
                
                this.AddEntry(key, child);
            }
        }

        public IEnumerator<string> GetEnumerator()
        {
            return this.children.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.children.Keys.GetEnumerator();
        }
    }
}