namespace Craiel.UnityEssentials.Runtime.Data.SBT
{
    using Nodes;
    using UnityEngine;

    public static class SBTReadExtensionsUnity
    {
        public static Vector2 ReadVector2(this SBTList source, int index)
        {
            return source.Read<SBTNodeVector2>(index).Data;
        }
        
        public static Vector2 ReadVector2(this SBTDictionary source, string key)
        {
            return source.Read<SBTNodeVector2>(key).Data;
        }
        
        public static Vector3 ReadVector3(this SBTList source, int index)
        {
            return source.Read<SBTNodeVector3>(index).Data;
        }
        
        public static Vector3 ReadVector3(this SBTDictionary source, string key)
        {
            return source.Read<SBTNodeVector3>(key).Data;
        }
        
        public static Quaternion ReadQuaternion(this SBTList source, int index)
        {
            return source.Read<SBTNodeQuaternion>(index).Data;
        }
        
        public static Quaternion ReadQuaternion(this SBTDictionary source, string key)
        {
            return source.Read<SBTNodeQuaternion>(key).Data;
        }
        
        public static Color ReadColor(this SBTList source, int index)
        {
            return source.Read<SBTNodeColor>(index).Data;
        }
        
        public static Color ReadColor(this SBTDictionary source, string key)
        {
            return source.Read<SBTNodeColor>(key).Data;
        }
    }
}