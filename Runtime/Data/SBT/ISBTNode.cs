namespace Craiel.UnityEssentials.Runtime.Data.SBT
{
    using System.IO;
    using Enums;

    public interface ISBTNode
    {
        SBTFlags Flags { get; }
        
        string Note { get; }
        
        SBTType Type { get; }

        void Save(BinaryWriter writer);
        void Load(BinaryReader reader);
    }
}