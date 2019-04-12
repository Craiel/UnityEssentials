namespace Craiel.UnityEssentials.Runtime.Data.SBT
{
    using System.IO;
    using Enums;
    using Nodes;

    public class SBTDictionary : SBTNodeDictionary
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static SBTDictionary Deserialize(string data)
        {
            return (SBTDictionary)SBTUtils.Deserialize(data);
        }
        
        public static SBTDictionary Deserialize(byte[] data)
        {
            return (SBTDictionary)SBTUtils.Deserialize(data);
        }
        
        public static SBTDictionary DeserializeCompressed(byte[] data)
        {
            return (SBTDictionary)SBTUtils.DeserializeCompressed(data);
        }
        
        public static SBTDictionary Deserialize(Stream source)
        {
            return (SBTDictionary)SBTUtils.Deserialize(source);
        }
    }
}