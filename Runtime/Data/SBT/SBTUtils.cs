namespace Craiel.UnityEssentials.Runtime.Data.SBT
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.IO.Compression;
    using System.Text;
    using Enums;
    using Nodes;

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
                case SBTType.ByteArray:
                case SBTType.ShortArray:
                case SBTType.UShortArray:
                case SBTType.IntArray:
                case SBTType.UIntArray:
                case SBTType.LongArray:
                case SBTType.ULongArray:
                case SBTType.SingleArray:
                case SBTType.DoubleArray:
                {
                    return new SBTNodeArray(type, flags);
                }

                case SBTType.List:
                {
                    return new SBTNodeList(flags);
                }

                case SBTType.Dictionary:
                {
                    return new SBTNodeDictionary(flags);
                }

                default:
                {
                    throw new InvalidDataException("Invalid Binary: " + type);
                }
            }
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
        
        public static void Serialize(this ISBTNode node, Stream target)
        {
            using (var writer = new BinaryWriter(target, Encoding.UTF8, true))
            {
                node.WriteHeader(writer);
                node.Serialize(writer);
            }
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
                        throw new InvalidDataException("SBT had unexpected root type: {0}" + type);
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

                default:
                {
                    throw new InvalidOperationException("Invalid SimpleType: " + type);
                }
            }
        }
    }
}