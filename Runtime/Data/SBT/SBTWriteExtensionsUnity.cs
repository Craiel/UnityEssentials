namespace Craiel.UnityEssentials.Runtime.Data.SBT
{
    using Enums;
    using Nodes;
    using UnityEngine;

    public static class SBTWriteExtensionsUnity
    {
        public static SBTNodeList Add(this SBTNodeList target, Vector2 data, SBTFlags flags = SBTFlags.None)
        {
            target.AddEntry(SBTType.Vector2, data, flags);
            return target;
        }
        
        public static SBTNodeDictionary Add(this SBTNodeDictionary target, string key, Vector2 data, SBTFlags flags = SBTFlags.None)
        {
            target.AddEntry(key, SBTType.Vector2, data, flags);
            return target;
        }
        
        public static SBTNodeList Add(this SBTNodeList target, Vector3 data, SBTFlags flags = SBTFlags.None)
        {
            target.AddEntry(SBTType.Vector3, data, flags);
            return target;
        }
        
        public static SBTNodeDictionary Add(this SBTNodeDictionary target, string key, Vector3 data, SBTFlags flags = SBTFlags.None)
        {
            target.AddEntry(key, SBTType.Vector3, data, flags);
            return target;
        }
        
        public static SBTNodeList Add(this SBTNodeList target, Quaternion data, SBTFlags flags = SBTFlags.None)
        {
            target.AddEntry(SBTType.Quaternion, data, flags);
            return target;
        }
        
        public static SBTNodeDictionary Add(this SBTNodeDictionary target, string key, Quaternion data, SBTFlags flags = SBTFlags.None)
        {
            target.AddEntry(key, SBTType.Quaternion, data, flags);
            return target;
        }
        
        public static SBTNodeList Add(this SBTNodeList target, Color data, SBTFlags flags = SBTFlags.None)
        {
            target.AddEntry(SBTType.Color, data, flags);
            return target;
        }
        
        public static SBTNodeDictionary Add(this SBTNodeDictionary target, string key, Color data, SBTFlags flags = SBTFlags.None)
        {
            target.AddEntry(key, SBTType.Color, data, flags);
            return target;
        }
    }
}