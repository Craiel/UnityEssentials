namespace Craiel.UnityEssentials.Runtime.Data.SBT
{
    using System;
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
        
        public static string ReadNote(this SBTList source, int index)
        {
            return source.Read(index).Note;
        }
        
        public static string ReadNote(this SBTDictionary source, string key)
        {
            return source.Read(key).Note;
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
        
        public static DateTime ReadDateTime(this SBTList source, int index)
        {
            return source.Read<SBTNodeDateTime>(index).Data;
        }
        
        public static DateTime ReadDateTime(this SBTDictionary source, string key)
        {
            return source.Read<SBTNodeDateTime>(key).Data;
        }
        
        public static TimeSpan ReadTimeSpan(this SBTList source, int index)
        {
            return source.Read<SBTNodeTimeSpan>(index).Data;
        }
        
        public static TimeSpan ReadTimeSpan(this SBTDictionary source, string key)
        {
            return source.Read<SBTNodeTimeSpan>(key).Data;
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
        
        public static string TryReadString(this SBTList source, int index, string defaultValue = default)
        {
            if (source.TryRead(index, out SBTNodeString node))
            {
                return node.Data;
            }

            return defaultValue;
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
        
        public static string TryReadString(this SBTDictionary source, string key, string defaultValue = default)
        {
            if (source.TryRead(key, out SBTNodeString node))
            {
                return node.Data;
            }

            return defaultValue;
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
        
        public static byte TryReadByte(this SBTList source, int index, byte defaultValue = default)
        {
            if (source.TryRead(index, out SBTNodeByte node))
            {
                return node.Data;
            }

            return defaultValue;
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
        
        public static byte TryReadByte(this SBTDictionary source, string key, byte defaultValue = default)
        {
            if (source.TryRead(key, out SBTNodeByte node))
            {
                return node.Data;
            }

            return defaultValue;
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
        
        public static short TryReadShort(this SBTList source, int index, short defaultValue = default)
        {
            if (source.TryRead(index, out SBTNodeShort node))
            {
                return node.Data;
            }

            return defaultValue;
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
        
        public static short TryReadShort(this SBTDictionary source, string key, short defaultValue = default)
        {
            if (source.TryRead(key, out SBTNodeShort node))
            {
                return node.Data;
            }

            return defaultValue;
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
        
        public static ushort TryReadUShort(this SBTList source, int index, ushort defaultValue = default)
        {
            if (source.TryRead(index, out SBTNodeUShort node))
            {
                return node.Data;
            }

            return default;
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
        
        public static ushort TryReadUShort(this SBTDictionary source, string key, ushort defaultValue = default)
        {
            if (source.TryRead(key, out SBTNodeUShort node))
            {
                return node.Data;
            }

            return defaultValue;
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

        public static int TryReadInt(this SBTList source, int index, int defaultValue = default)
        {
            if (source.TryRead(index, out SBTNodeInt node))
            {
                return node.Data;
            }

            return defaultValue;
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

        public static int TryReadInt(this SBTDictionary source, string key, int defaultValue = default)
        {
            if (source.TryRead(key, out SBTNodeInt node))
            {
                return node.Data;
            }

            return defaultValue;
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
        
        public static uint TryReadUInt(this SBTList source, int index, uint defaultValue = default)
        {
            if (source.TryRead(index, out SBTNodeUInt node))
            {
                return node.Data;
            }

            return defaultValue;
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
        
        public static uint TryReadUInt(this SBTDictionary source, string key, uint defaultValue = default)
        {
            if (source.TryRead(key, out SBTNodeUInt node))
            {
                return node.Data;
            }

            return defaultValue;
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
        
        public static long TryReadLong(this SBTList source, int index, long defaultValue = default)
        {
            if (source.TryRead(index, out SBTNodeLong node))
            {
                return node.Data;
            }

            return defaultValue;
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
        
        public static long TryReadLong(this SBTDictionary source, string key, long defaultValue = default)
        {
            if (source.TryRead(key, out SBTNodeLong node))
            {
                return node.Data;
            }

            return defaultValue;
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
        
        public static ulong TryReadULong(this SBTList source, int index, ulong defaultValue = default)
        {
            if (source.TryRead(index, out SBTNodeULong node))
            {
                return node.Data;
            }

            return defaultValue;
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
        
        public static ulong TryReadULong(this SBTDictionary source, string key, ulong defaultValue = default)
        {
            if (source.TryRead(key, out SBTNodeULong node))
            {
                return node.Data;
            }

            return defaultValue;
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
        
        public static float TryReadSingle(this SBTList source, int index, float defaultValue = default)
        {
            if (source.TryRead(index, out SBTNodeSingle node))
            {
                return node.Data;
            }

            return defaultValue;
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
        
        public static float TryReadSingle(this SBTDictionary source, string key, float defaultValue = default)
        {
            if (source.TryRead(key, out SBTNodeSingle node))
            {
                return node.Data;
            }

            return defaultValue;
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
        
        public static double TryReadDouble(this SBTList source, int index, double defaultValue = default)
        {
            if (source.TryRead(index, out SBTNodeDouble node))
            {
                return node.Data;
            }

            return defaultValue;
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
        
        public static double TryReadDouble(this SBTDictionary source, string key, double defaultValue = default)
        {
            if (source.TryRead(key, out SBTNodeDouble node))
            {
                return node.Data;
            }

            return defaultValue;
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
        
        public static SBTFlags TryReadFlags(this SBTList source, int index, SBTFlags defaultValue = default)
        {
            if (source.TryRead(index, out ISBTNode node))
            {
                return node.Flags;
            }

            return defaultValue;
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
        
        public static SBTFlags TryReadFlags(this SBTDictionary source, string key, SBTFlags defaultValue = default)
        {
            if (source.TryRead(key, out ISBTNode node))
            {
                return node.Flags;
            }

            return defaultValue;
        }

        public static bool TryReadArray<T>(this SBTList source, int index, out SBTNodeArray<T> result)
        {
            return source.TryRead(index, out result);
        }
        
        public static SBTNodeArray<T> TryReadArray<T>(this SBTList source, int index, SBTNodeArray<T> defaultValue = default)
        {
            return source.TryRead(index, out SBTNodeArray<T> result) ? result : defaultValue;
        }
        
        public static bool TryReadArray<T>(this SBTDictionary source, string key, out SBTNodeArray<T> result)
        {
            return source.TryRead(key, out result);
        }
        
        public static SBTNodeArray<T> TryReadArray<T>(this SBTDictionary source, string key, SBTNodeArray<T> defaultValue = default)
        {
            return source.TryRead(key, out SBTNodeArray<T> result) ? result : defaultValue;
        }
        
        public static bool TryReadList(this SBTList source, int index, out SBTNodeList result)
        {
            return source.TryRead(index, out result);
        }
        
        public static SBTNodeList TryReadList(this SBTList source, int index, SBTNodeList defaultValue)
        {
            return source.TryRead(index, out SBTNodeList result) ? result : defaultValue;
        }
        
        public static bool TryReadList(this SBTDictionary source, string key, out SBTNodeList result)
        {
            return source.TryRead(key, out result);
        }
        
        public static SBTNodeList TryReadList(this SBTDictionary source, string key, SBTNodeList defaultValue)
        {
            return source.TryRead(key, out SBTNodeList result) ? result : defaultValue;
        }
        
        public static bool TryReadDictionary(this SBTList source, int index, out SBTNodeDictionary result)
        {
            return source.TryRead(index, out result);
        }
        
        public static SBTNodeDictionary TryReadDictionary(this SBTList source, int index, SBTNodeDictionary defaultValue)
        {
            return source.TryRead(index, out SBTNodeDictionary result) ? result : defaultValue;
        }
        
        public static bool TryReadDictionary(this SBTDictionary source, string key, out SBTNodeDictionary result)
        {
            return source.TryRead(key, out result);
        }
        
        public static SBTNodeDictionary TryReadDictionary(this SBTDictionary source, string key, SBTNodeDictionary defaultValue)
        {
            return source.TryRead(key, out SBTNodeDictionary result) ? result : defaultValue;
        }
        
        public static bool TryReadStream(this SBTList source, int index, out SBTNodeStream result)
        {
            return source.TryRead(index, out result);
        }
        
        public static SBTNodeStream TryReadStream(this SBTList source, int index, SBTNodeStream defaultValue)
        {
            return source.TryRead(index, out SBTNodeStream result) ? result : defaultValue;
        }
        
        public static bool TryReadStream(this SBTDictionary source, string key, out SBTNodeStream result)
        {
            return source.TryRead(key, out result);
        }
        
        public static SBTNodeStream TryReadStream(this SBTDictionary source, string key, SBTNodeStream defaultValue)
        {
            return source.TryRead(key, out SBTNodeStream result) ? result : defaultValue;
        }
        
        public static bool TryReadDateTime(this SBTList source, int index, out DateTime result)
        {
            result = default;
            if (source.TryRead(index, out SBTNodeDateTime node))
            {
                result = node.Data;
                return true;
            }

            return false;
        }
        
        public static DateTime TryReadDateTime(this SBTList source, int index, DateTime defaultValue = default)
        {
            if (source.TryRead(index, out SBTNodeDateTime node))
            {
                return node.Data;
            }

            return defaultValue;
        }
        
        public static bool TryReadDateTime(this SBTDictionary source, string key, out DateTime result)
        {
            result = default;
            if (source.TryRead(key, out SBTNodeDateTime node))
            {
                result = node.Data;
                return true;
            }

            return false;
        }
        
        public static DateTime TryReadDateTime(this SBTDictionary source, string key, DateTime defaultValue = default)
        {
            if (source.TryRead(key, out SBTNodeDateTime node))
            {
                return node.Data;
            }

            return defaultValue;
        }
        
        public static bool TryReadTimeSpan(this SBTList source, int index, out TimeSpan result)
        {
            result = default;
            if (source.TryRead(index, out SBTNodeTimeSpan node))
            {
                result = node.Data;
                return true;
            }

            return false;
        }
        
        public static TimeSpan TryReadTimeSpan(this SBTList source, int index, TimeSpan defaultValue = default)
        {
            if (source.TryRead(index, out SBTNodeTimeSpan node))
            {
                return node.Data;
            }

            return defaultValue;
        }
        
        public static bool TryReadTimeSpan(this SBTDictionary source, string key, out TimeSpan result)
        {
            result = default;
            if (source.TryRead(key, out SBTNodeTimeSpan node))
            {
                result = node.Data;
                return true;
            }

            return false;
        }
        
        public static TimeSpan TryReadTimeSpan(this SBTDictionary source, string key, TimeSpan defaultValue = default)
        {
            if (source.TryRead(key, out SBTNodeTimeSpan node))
            {
                return node.Data;
            }

            return defaultValue;
        }
    }
}