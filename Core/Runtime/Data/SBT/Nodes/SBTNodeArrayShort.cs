namespace Craiel.UnityEssentials.Runtime.Data.SBT.Nodes
{
    using System.IO;
    using Enums;

    public class SBTNodeArrayShort : SBTNodeArray<short>
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SBTNodeArrayShort(SBTType type, SBTFlags flags, string note = null) 
            : base(type, flags, note)
        {
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void SerializeOne(BinaryWriter writer, short entry)
        {
            writer.Write(entry);
        }

        protected override short DeserializeOne(BinaryReader reader)
        {
            return reader.ReadInt16();
        }
    }
}