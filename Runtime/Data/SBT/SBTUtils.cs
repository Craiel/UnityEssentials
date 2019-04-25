namespace Craiel.UnityEssentials.Runtime.Data.SBT
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.IO.Compression;
    using System.Text;
    using Enums;
    using Extensions;
    using IO;
    using Nodes;
    using UnityEngine;
    using CompressionLevel = System.IO.Compression.CompressionLevel;

    public static class SBTUtils
    {
        private const int CompressionWarningThreshold = 256;
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public static ISBTNode GetNode(SBTType type, object data = null, SBTFlags flags = SBTFlags.None)
        {
            if (type.IsSimpleType() && data == null)
            {
                throw new InvalidOperationException("Data must be set for simple types");
            }
            
            switch (type)
            {
                case SBTType.String:
                {
                    return new SBTNodeString((string)data, flags);
                }

                case SBTType.Byte:
                {
                    return new SBTNodeByte((byte)data, flags);
                }

                case SBTType.Short:
                {
                    return new SBTNodeShort((short)data, flags);
                }

                case SBTType.UShort:
                {
                    return new SBTNodeUShort((ushort)data, flags);
                }

                case SBTType.Int:
                {
                    return new SBTNodeInt((int)data, flags);
                }

                case SBTType.UInt:
                {
                    return new SBTNodeUInt((uint)data, flags);
                }

                case SBTType.Long:
                {
                    return new SBTNodeLong((long)data, flags);
                }

                case SBTType.ULong:
                {
                    return new SBTNodeULong((ulong)data, flags);
                }

                case SBTType.Single:
                {
                    return new SBTNodeSingle((float)data, flags);
                }
                
                case SBTType.Double:
                {
                    return new SBTNodeDouble((double)data, flags);
                }

                case SBTType.StringArray:
                {
                    return new SBTNodeArrayString(type, flags);
                }
                
                case SBTType.ByteArray:
                {
                    return new SBTNodeArrayByte(type, flags);
                }
                
                case SBTType.ShortArray:
                {
                    return new SBTNodeArrayShort(type, flags);
                }
                
                case SBTType.UShortArray:
                {
                    return new SBTNodeArrayUShort(type, flags);
                }
                
                case SBTType.IntArray:
                {
                    return new SBTNodeArrayInt(type, flags);
                }
                
                case SBTType.UIntArray:
                {
                    return new SBTNodeArrayUInt(type, flags);
                }
                
                case SBTType.LongArray:
                {
                    return new SBTNodeArrayLong(type, flags);
                }
                
                case SBTType.ULongArray:
                {
                    return new SBTNodeArrayULong(type, flags);
                }
                
                case SBTType.SingleArray:
                {
                    return new SBTNodeArraySingle(type, flags);
                }
                
                case SBTType.DoubleArray:
                {
                    return new SBTNodeArrayDouble(type, flags);
                }

                case SBTType.List:
                {
                    return new SBTNodeList(flags);
                }

                case SBTType.Dictionary:
                {
                    return new SBTNodeDictionary(flags);
                }

                case SBTType.Stream:
                {
                    return new SBTNodeStream();
                }

                case SBTType.Vector2:
                {
                    return new SBTNodeVector2((Vector2)data, flags);
                }
                
                case SBTType.Vector3:
                {
                    return new SBTNodeVector3((Vector3)data, flags);
                }
                
                case SBTType.Quaternion:
                {
                    return new SBTNodeQuaternion((Quaternion)data, flags);
                }
                
                case SBTType.Color:
                {
                    return new SBTNodeColor((Color)data, flags);
                }

                case SBTType.DateTime:
                {
                    return new SBTNodeDateTime((DateTime) data, flags);
                }

                case SBTType.TimeSpan:
                {
                    return new SBTNodeTimeSpan((TimeSpan) data, flags);
                }

                default:
                {
                    throw new InvalidDataException("Invalid Binary: " + type);
                }
            }
        }
        
        public static string SerializeToString(this ISBTNode node)
        {
            byte[] data = node.Serialize();
            return Convert.ToBase64String(data);
        }
        
        public static byte[] Serialize(this ISBTNode node)
        {
            using (var stream = new MemoryStream())
            {
                node.Serialize(stream);
                
                byte[] data = new byte[stream.Length];
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(data, 0, data.Length);
                return data;
            }
        }

        public static byte[] SerializeCompressed(this ISBTNode node)
        {
            using (var stream = new MemoryStream())
            {
                byte[] rawData = Serialize(node);
                if (rawData.Length <= CompressionWarningThreshold)
                {
                    EssentialsCore.Logger.Warn("SBT Compressed Serialize called on small data, use compression on large data sets only");
                }
                
                using(var zipStream = new GZipStream(stream, CompressionLevel.Optimal, true))
                {
                    zipStream.Write(rawData, 0, rawData.Length);
                    zipStream.Flush();
                }
                
                byte[] data = new byte[stream.Length];
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(data, 0, data.Length);
                return data;
            }
        }

        public static void SerializeToFile(this ISBTNode node, ManagedFile target)
        {
            target.GetDirectory().Create();
            using (var stream = target.OpenWrite())
            {
                byte[] data = node.Serialize();
                stream.Write(data, 0, data.Length);
            }
        }
        
        public static void SerializeToFileCompressed(this ISBTNode node, ManagedFile target)
        {
            target.GetDirectory().Create();
            using (var stream = target.OpenWrite())
            {
                byte[] data = node.SerializeCompressed();
                stream.Write(data, 0, data.Length);
            }
        }
        
        public static void Serialize(this ISBTNode node, Stream target)
        {
            using (var writer = new BinaryWriter(target, Encoding.UTF8, true))
            {
                node.WriteHeader(writer);
                node.Serialize(writer);
            }
        }
        
        public static ISBTNode Deserialize(string data)
        {
            return Deserialize(Convert.FromBase64String(data));
        }
        
        public static ISBTNode Deserialize(byte[] data)
        {
            using (var stream = new MemoryStream(data))
            {
                return Deserialize(stream);
            }
        }
        
        public static ISBTNode DeserializeCompressed(byte[] data)
        {
            using (var stream = new MemoryStream(data))
            {
                using (var decompressedStream = new MemoryStream())
                {
                    using (var zipStream = new GZipStream(stream, CompressionMode.Decompress, true))
                    {
                        zipStream.CopyTo(decompressedStream);
                    }

                    decompressedStream.Seek(0, SeekOrigin.Begin);
                    return Deserialize(decompressedStream);
                }
            }
        }
        
        public static ISBTNode DeserializeCompressed(Stream stream)
        {
            using (var decompressedStream = new MemoryStream())
            {
                using (var zipStream = new GZipStream(stream, CompressionMode.Decompress, true))
                {
                    zipStream.CopyTo(decompressedStream);
                }

                decompressedStream.Seek(0, SeekOrigin.Begin);
                return Deserialize(decompressedStream);
            }
        }

        public static ISBTNode Deserialize(Stream stream)
        {
            using (var reader = new BinaryReader(stream, Encoding.UTF8, true))
            {
                SBTType type;
                SBTFlags flags;
                ReadHeader(reader, out type, out flags);

                switch (type)
                {
                    case SBTType.List:
                    {
                        var result = new SBTList();
                        result.Deserialize(reader);
                        return result;
                    }

                    case SBTType.Dictionary:
                    {
                        var result = new SBTDictionary();
                        result.Deserialize(reader);
                        return result;
                    }

                    default:
                    {
                        throw new InvalidDataException("SBT had unexpected root type: " + type);
                    }
                }
            }
        }
        
        internal static void WriteHeader(this ISBTNode node, BinaryWriter target)
        {
            target.Write((byte)node.Type);
            target.Write((ushort)node.Flags);
        }
        
        internal static void ReadHeader(BinaryReader source, out SBTType nodeType, out SBTFlags nodeFlags)
        {
            nodeType = (SBTType)source.ReadByte();
            nodeFlags = (SBTFlags)source.ReadUInt16();
        }
        
        public static object ReadSimpleTypeData(SBTType type, BinaryReader reader)
        {
            switch (type)
            {
                case SBTType.String:
                {
                    return reader.ReadString();
                }
                
                case SBTType.Byte:
                {
                    return reader.ReadByte();
                }

                case SBTType.Short:
                {
                    return reader.ReadInt16();
                }

                case SBTType.UShort:
                {
                    return reader.ReadUInt16();
                }

                case SBTType.Int:
                {
                    return reader.ReadInt32();
                }

                case SBTType.UInt:
                {
                    return reader.ReadUInt32();
                }

                case SBTType.Long:
                {
                    return reader.ReadInt64();
                }

                case SBTType.ULong:
                {
                    return reader.ReadUInt64();
                }

                case SBTType.Single:
                {
                    return reader.ReadSingle();
                }

                case SBTType.Double:
                {
                    return reader.ReadDouble();
                }

                case SBTType.Vector2:
                {
                    return reader.ReadVector2();
                }

                case SBTType.Vector3:
                {
                    return reader.ReadVector3();
                }

                case SBTType.Quaternion:
                {
                    return reader.ReadQuaternion();
                }

                case SBTType.Color:
                {
                    return reader.ReadColor();
                }

                case SBTType.DateTime:
                {
                    return reader.ReadDateTime();
                }

                case SBTType.TimeSpan:
                {
                    return reader.ReadTimeSpan();
                }

                default:
                {
                    throw new InvalidOperationException("Invalid SimpleType: " + type);
                }
            }
        }

        public static byte? GetDataEntrySize(SBTType type)
        {
            switch (type)
            {
                case SBTType.Byte:
                {
                    return sizeof(byte);
                }

                case SBTType.Short:
                {
                    return sizeof(short);
                }

                case SBTType.UShort:
                {
                    return sizeof(ushort);
                }

                case SBTType.Int:
                {
                    return sizeof(int);
                }

                case SBTType.UInt:
                {
                    return sizeof(uint);
                }

                case SBTType.Long:
                {
                    return sizeof(long);
                }

                case SBTType.ULong:
                {
                    return sizeof(ulong);
                }

                case SBTType.Single:
                {
                    return sizeof(float);
                }

                case SBTType.Double:
                {
                    return sizeof(double);
                }

                case SBTType.Vector2:
                {
                    return sizeof(float) * 2;
                }

                case SBTType.Vector3:
                {
                    return sizeof(float) * 3;
                }

                case SBTType.Quaternion:
                {
                    return sizeof(float) * 4;
                }

                case SBTType.Color:
                {
                    return sizeof(float) * 4;
                }

                case SBTType.DateTime:
                case SBTType.TimeSpan:
                {
                    return sizeof(long);
                }

                default:
                {
                    return null;
                }
            }
        }

        public static SBTType GetArrayBaseType(Type type)
        {
            if (type == typeof(string))
            {
                return SBTType.StringArray;
            }

            if (type == typeof(byte))
            {
                return SBTType.ByteArray;
            }

            if (type == typeof(short))
            {
                return SBTType.ShortArray;
            }

            if (type == typeof(ushort))
            {
                return SBTType.UShortArray;
            }

            if (type == typeof(int))
            {
                return SBTType.IntArray;
            }

            if (type == typeof(uint))
            {
                return SBTType.UIntArray;
            }

            if (type == typeof(long))
            {
                return SBTType.LongArray;
            }

            if (type == typeof(ulong))
            {
                return SBTType.ULongArray;
            }

            if (type == typeof(float))
            {
                return SBTType.SingleArray;
            }

            if (type == typeof(double))
            {
                return SBTType.DoubleArray;
            }
            
            throw new ArgumentException("Type not supported for SBT Array");
        }
    }
}