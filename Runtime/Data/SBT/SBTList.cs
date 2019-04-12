namespace Craiel.UnityEssentials.Runtime.Data.SBT
{
    using System.IO;
    using Enums;
    using Nodes;

    public class SBTList : SBTNodeList
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static SBTList Deserialize(string data)
        {
            return (SBTList)SBTUtils.Deserialize(data);
        }
        
        public static SBTList Deserialize(byte[] data)
        {
            return (SBTList)SBTUtils.Deserialize(data);
        }
        
        public static SBTList DeserializeCompressed(byte[] data)
        {
            return (SBTList)SBTUtils.DeserializeCompressed(data);
        }
        
        public static SBTList Deserialize(Stream source)
        {
            return (SBTList)SBTUtils.Deserialize(source);
        }
    }
}