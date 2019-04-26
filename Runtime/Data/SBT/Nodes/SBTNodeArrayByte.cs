namespace Craiel.UnityEssentials.Runtime.Data.SBT.Nodes
{
    using System.IO;
    using Enums;

    public class SBTNodeArrayByte : SBTNodeArray<byte>
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SBTNodeArrayByte(SBTType type, SBTFlags flags, string note = null) 
            : base(type, flags, note)
        {
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void SerializeOne(BinaryWriter writer, byte entry)
        {
            writer.Write(entry);
        }

        protected override byte DeserializeOne(BinaryReader reader)
        {
            return reader.ReadByte();
        }
    }
}