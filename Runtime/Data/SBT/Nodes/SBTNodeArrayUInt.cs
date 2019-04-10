namespace Craiel.UnityEssentials.Runtime.Data.SBT.Nodes
{
    using System.IO;
    using Enums;

    public class SBTNodeArrayUInt : SBTNodeArray<uint>
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SBTNodeArrayUInt(SBTType type, SBTFlags flags) 
            : base(type, flags)
        {
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void SerializeOne(BinaryWriter writer, uint entry)
        {
            writer.Write(entry);
        }

        protected override uint DeserializeOne(BinaryReader reader)
        {
            return reader.ReadUInt32();
        }
    }
}