namespace Craiel.UnityEssentials.Runtime.Data.SBT
{
    using Enums;
    using Nodes;

    public static class SBTReadExtensions
    {
        public static string ReadString(this SBTList source, int index)
        {
            return source.Read<SBTNodeString>(index).Data;
        }
        
        public static string ReadString(this SBTDictionary source, string key)
        {
            return source.Read<SBTNodeString>(key).Data;
        }
        
        public static byte ReadByte(this SBTList source, int index)
        {
            return source.Read<SBTNodeByte>(index).Data;
        }
        
        public static byte ReadByte(this SBTDictionary source, string key)
        {
            return source.Read<SBTNodeByte>(key).Data;
        }
        
        public static short ReadShort(this SBTList source, int index)
        {
            return source.Read<SBTNodeShort>(index).Data;
        }
        
        public static short ReadShort(this SBTDictionary source, string key)
        {
            return source.Read<SBTNodeShort>(key).Data;
        }
        
        public static ushort ReadUShort(this SBTList source, int index)
        {
            return source.Read<SBTNodeUShort>(index).Data;
        }
        
        public static ushort ReadUShort(this SBTDictionary source, string key)
        {
            return source.Read<SBTNodeUShort>(key).Data;
        }
        
        public static int ReadInt(this SBTList source, int index)
        {
            return source.Read<SBTNodeInt>(index).Data;
        }
        
        public static int ReadInt(this SBTDictionary source, string key)
        {
            return source.Read<SBTNodeInt>(key).Data;
        }
        
        public static uint ReadUInt(this SBTList source, int index)
        {
            return source.Read<SBTNodeUInt>(index).Data;
        }
        
        public static uint ReadUInt(this SBTDictionary source, string key)
        {
            return source.Read<SBTNodeUInt>(key).Data;
        }
        
        public static long ReadLong(this SBTList source, int index)
        {
            return source.Read<SBTNodeLong>(index).Data;
        }
        
        public static long ReadLong(this SBTDictionary source, string key)
        {
            return source.Read<SBTNodeLong>(key).Data;
        }
        
        public static ulong ReadULong(this SBTList source, int index)
        {
            return source.Read<SBTNodeULong>(index).Data;
        }
        
        public static ulong ReadULong(this SBTDictionary source, string key)
        {
            return source.Read<SBTNodeULong>(key).Data;
        }
        
        public static float ReadSingle(this SBTList source, int index)
        {
            return source.Read<SBTNodeSingle>(index).Data;
        }
        
        public static float ReadSingle(this SBTDictionary source, string key)
        {
            return source.Read<SBTNodeSingle>(key).Data;
        }
        
        public static double ReadDouble(this SBTList source, int index)
        {
            return source.Read<SBTNodeDouble>(index).Data;
        }
        
        public static double ReadDouble(this SBTDictionary source, string key)
        {
            return source.Read<SBTNodeDouble>(key).Data;
        }

        public static SBTFlags ReadFlags(this SBTList source, int index)
        {
            return source.Read(index).Flags;
        }
        
        public static SBTFlags ReadFlags(this SBTDictionary source, string key)
        {
            return source.Read(key).Flags;
        }

        public static SBTNodeArray<T> ReadArray<T>(this SBTList source, int index)
        {
            return source.Read<SBTNodeArray<T>>(index);
        }
        
        public static SBTNodeArray<T> ReadArray<T>(this SBTDictionary source, string key)
        {
            return source.Read<SBTNodeArray<T>>(key);
        }
        
        public static SBTNodeList ReadList(this SBTList source, int index)
        {
            return source.Read<SBTNodeList>(index);
        }
        
        public static SBTNodeList ReadList(this SBTDictionary source, string key)
        {
            return source.Read<SBTNodeList>(key);
        }
        
        public static SBTNodeDictionary ReadDictionary(this SBTList source, int index)
        {
            return source.Read<SBTNodeDictionary>(index);
        }
        
        public static SBTNodeDictionary ReadDictionary(this SBTDictionary source, string key)
        {
            return source.Read<SBTNodeDictionary>(key);
        }
        
        public static SBTNodeStream ReadStream(this SBTList source, int index)
        {
            return source.Read<SBTNodeStream>(index);
        }
        
        public static SBTNodeStream ReadStream(this SBTDictionary source, string key)
        {
            return source.Read<SBTNodeStream>(key);
        }
    }
}