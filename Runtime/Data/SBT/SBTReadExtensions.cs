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
        
        public static bool TryReadString(this SBTList source, int index, out string result)
        {
            result = null;
            if (source.TryRead(index, out SBTNodeString node))
            {
                result = node.Data;
                return true;
            }

            return false;
        }
        
        public static bool TryReadString(this SBTDictionary source, string key, out string result)
        {
            result = null;
            if (source.TryRead(key, out SBTNodeString node))
            {
                result = node.Data;
                return true;
            }

            return false;
        }
        
        public static bool TryReadByte(this SBTList source, int index, out byte result)
        {
            result = default;
            if (source.TryRead(index, out SBTNodeByte node))
            {
                result = node.Data;
                return true;
            }

            return false;
        }
        
        public static bool TryReadByte(this SBTDictionary source, string key, out byte result)
        {
            result = default;
            if (source.TryRead(key, out SBTNodeByte node))
            {
                result = node.Data;
                return true;
            }

            return false;
        }
        
        public static bool TryReadShort(this SBTList source, int index, out short result)
        {
            result = default;
            if (source.TryRead(index, out SBTNodeShort node))
            {
                result = node.Data;
                return true;
            }

            return false;
        }
        
        public static bool TryReadShort(this SBTDictionary source, string key, out short result)
        {
            result = default;
            if (source.TryRead(key, out SBTNodeShort node))
            {
                result = node.Data;
                return true;
            }

            return false;
        }
        
        public static bool TryReadUShort(this SBTList source, int index, out ushort result)
        {
            result = default;
            if (source.TryRead(index, out SBTNodeUShort node))
            {
                result = node.Data;
                return true;
            }

            return false;
        }
        
        public static bool TryReadUShort(this SBTDictionary source, string key, out ushort result)
        {
            result = default;
            if (source.TryRead(key, out SBTNodeUShort node))
            {
                result = node.Data;
                return true;
            }

            return false;
        }
        
        public static bool TryReadInt(this SBTList source, int index, out int result)
        {
            result = default;
            if (source.TryRead(index, out SBTNodeInt node))
            {
                result = node.Data;
                return true;
            }

            return false;
        }
        
        public static bool TryReadInt(this SBTDictionary source, string key, out int result)
        {
            result = default;
            if (source.TryRead(key, out SBTNodeInt node))
            {
                result = node.Data;
                return true;
            }

            return false;
        }
        
        public static bool TryReadUInt(this SBTList source, int index, out uint result)
        {
            result = default;
            if (source.TryRead(index, out SBTNodeUInt node))
            {
                result = node.Data;
                return true;
            }

            return false;
        }
        
        public static bool TryReadUInt(this SBTDictionary source, string key, out uint result)
        {
            result = default;
            if (source.TryRead(key, out SBTNodeUInt node))
            {
                result = node.Data;
                return true;
            }

            return false;
        }
        
        public static bool TryReadLong(this SBTList source, int index, out long result)
        {
            result = default;
            if (source.TryRead(index, out SBTNodeLong node))
            {
                result = node.Data;
                return true;
            }

            return false;
        }
        
        public static bool TryReadLong(this SBTDictionary source, string key, out long result)
        {
            result = default;
            if (source.TryRead(key, out SBTNodeLong node))
            {
                result = node.Data;
                return true;
            }

            return false;
        }
        
        public static bool TryReadULong(this SBTList source, int index, out ulong result)
        {
            result = default;
            if (source.TryRead(index, out SBTNodeULong node))
            {
                result = node.Data;
                return true;
            }

            return false;
        }
        
        public static bool TryReadULong(this SBTDictionary source, string key, out ulong result)
        {
            result = default;
            if (source.TryRead(key, out SBTNodeULong node))
            {
                result = node.Data;
                return true;
            }

            return false;
        }
        
        public static bool TryReadSingle(this SBTList source, int index, out float result)
        {
            result = default;
            if (source.TryRead(index, out SBTNodeSingle node))
            {
                result = node.Data;
                return true;
            }

            return false;
        }
        
        public static bool TryReadSingle(this SBTDictionary source, string key, out float result)
        {
            result = default;
            if (source.TryRead(key, out SBTNodeSingle node))
            {
                result = node.Data;
                return true;
            }

            return false;
        }
        
        public static bool TryReadDouble(this SBTList source, int index, out double result)
        {
            result = default;
            if (source.TryRead(index, out SBTNodeDouble node))
            {
                result = node.Data;
                return true;
            }

            return false;
        }
        
        public static bool TryReadDouble(this SBTDictionary source, string key, out double result)
        {
            result = default;
            if (source.TryRead(key, out SBTNodeDouble node))
            {
                result = node.Data;
                return true;
            }

            return false;
        }

        public static bool TryReadFlags(this SBTList source, int index, out SBTFlags result)
        {
            result = default;
            if (source.TryRead(index, out ISBTNode node))
            {
                result = node.Flags;
                return true;
            }

            return false;
        }
        
        public static bool TryReadFlags(this SBTDictionary source, string key, out SBTFlags result)
        {
            result = default;
            if (source.TryRead(key, out ISBTNode node))
            {
                result = node.Flags;
                return true;
            }

            return false;
        }

        public static bool TryReadArray<T>(this SBTList source, int index, out SBTNodeArray<T> result)
        {
            return source.TryRead(index, out result);
        }
        
        public static bool TryReadArray<T>(this SBTDictionary source, string key, out SBTNodeArray<T> result)
        {
            return source.TryRead(key, out result);
        }
        
        public static bool TryReadList(this SBTList source, int index, out SBTNodeList result)
        {
            return source.TryRead(index, out result);
        }
        
        public static bool TryReadList(this SBTDictionary source, string key, out SBTNodeList result)
        {
            return source.TryRead(key, out result);
        }
        
        public static bool TryReadDictionary(this SBTList source, int index, out SBTNodeDictionary result)
        {
            return source.TryRead(index, out result);
        }
        
        public static bool TryReadDictionary(this SBTDictionary source, string key, out SBTNodeDictionary result)
        {
            return source.TryRead(key, out result);
        }
        
        public static bool TryReadStream(this SBTList source, int index, out SBTNodeStream result)
        {
            return source.TryRead(index, out result);
        }
        
        public static bool TryReadStream(this SBTDictionary source, string key, out SBTNodeStream result)
        {
            return source.TryRead(key, out result);
        }
    }
}