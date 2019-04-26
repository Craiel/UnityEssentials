namespace Craiel.UnityEssentials.Runtime.Data.SBT.Nodes
{
    using System.IO;
    using Enums;

    public class SBTNodeArraySingle : SBTNodeArray<float>
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SBTNodeArraySingle(SBTType type, SBTFlags flags, string note = null) 
            : base(type, flags, note)
        {
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void SerializeOne(BinaryWriter writer, float entry)
        {
            writer.Write(entry);
        }

        protected override float DeserializeOne(BinaryReader reader)
        {
            return reader.ReadSingle();
        }
    }
}