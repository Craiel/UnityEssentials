namespace Craiel.UnityEssentials.Runtime.Data.SBT
{
    using System.IO;
    using Enums;

    public interface ISBTNode
    {
        SBTFlags Flags { get; }
        
        SBTType Type { get; }

        void Serialize(BinaryWriter writer);
        void Deserialize(BinaryReader reader);
    }
}