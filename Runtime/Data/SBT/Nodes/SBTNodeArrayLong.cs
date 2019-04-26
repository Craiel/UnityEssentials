namespace Craiel.UnityEssentials.Runtime.Data.SBT.Nodes
{
    using System.IO;
    using Enums;

    public class SBTNodeArrayLong : SBTNodeArray<long>
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SBTNodeArrayLong(SBTType type, SBTFlags flags, string note = null) 
            : base(type, flags, note)
        {
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void SerializeOne(BinaryWriter writer, long entry)
        {
            writer.Write(entry);
        }

        protected override long DeserializeOne(BinaryReader reader)
        {
            return reader.ReadInt64();
        }
    }
}