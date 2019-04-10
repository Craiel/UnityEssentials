namespace Craiel.UnityEssentials.Runtime.Data.SBT.Nodes
{
    using System.IO;
    using Enums;

    public class SBTNodeArrayDouble : SBTNodeArray<double>
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SBTNodeArrayDouble(SBTType type, SBTFlags flags) 
            : base(type, flags)
        {
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void SerializeOne(BinaryWriter writer, double entry)
        {
            writer.Write(entry);
        }

        protected override double DeserializeOne(BinaryReader reader)
        {
            return reader.ReadDouble();
        }
    }
}