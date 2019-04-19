namespace Craiel.UnityEssentials.Runtime.Data.SBT.Nodes
{
    using System;
    using System.IO;
    using System.Text;
    using Enums;

    public class SBTNodeStream : ISBTNode
    {
        private readonly MemoryStream inner;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SBTNodeStream(SBTFlags flags = SBTFlags.None)
        {
            this.Flags = flags;
            this.inner = new MemoryStream();
            this.Encoding = Encoding.UTF8;
        }
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public Encoding Encoding { get; set; }
        
        public SBTFlags Flags { get; }

        public long Length
        {
            get { return this.inner.Length; }
        }

        public SBTType Type
        {
            get { return SBTType.Stream; }
        }
        
        public void Serialize(BinaryWriter writer)
        {
            this.inner.Seek(0, SeekOrigin.Begin);
            byte[] data = new byte[this.inner.Length];
            this.inner.Read(data, 0, data.Length);
            this.inner.Seek(0, SeekOrigin.End);
            
            writer.Write(data.Length);
            writer.Write(data);
        }
        
        public void Deserialize(BinaryReader reader)
        {
            int length = reader.ReadInt32();
            byte[] data = reader.ReadBytes(length);
            this.inner.Seek(0, SeekOrigin.Begin);
            this.inner.Write(data, 0, data.Length);
            this.inner.Seek(0, SeekOrigin.Begin);
        }

        public BinaryReader BeginRead()
        {
            return new BinaryReader(this.inner, this.Encoding, true);
        }

        public BinaryWriter BeginWrite()
        {
            return new BinaryWriter(this.inner, this.Encoding, true);
        }
        
        public void Seek(int target, SeekOrigin origin)
        {
            this.inner.Seek(target, origin);
        }
        
        public void Flush()
        {
            this.inner.Flush();
        }
    }
}