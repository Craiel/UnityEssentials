namespace Craiel.UnityEssentials.Runtime.Data.SBT
{
    using System.IO;
    using Enums;

    public static class SBTTypeExtensions
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static bool IsSimpleType(this SBTType type)
        {
            switch (type)
            {
                case SBTType.String:
                case SBTType.Byte:
                case SBTType.Short:
                case SBTType.UShort:
                case SBTType.Int:
                case SBTType.UInt:
                case SBTType.Long:
                case SBTType.ULong:
                case SBTType.Single:
                case SBTType.Double:
                case SBTType.Vector2:
                case SBTType.Vector3:
                case SBTType.Quaternion:
                case SBTType.Color:
                case SBTType.DateTime:
                case SBTType.TimeSpan:
                {
                    return true;
                }

                default:
                {
                    return false;
                }
            }
        }
        
        public static bool IsArrayType(this SBTType type)
        {
            switch (type)
            {
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
                    return true;
                }

                default:
                {
                    return false;
                }
            }
        }
        
        public static SBTType GetArrayBaseType(this SBTType type)
        {
            switch (type)
            {
                case SBTType.StringArray:
                {
                    return SBTType.String;
                }
                    
                case SBTType.ByteArray:
                {
                    return SBTType.Byte;
                }

                case SBTType.ShortArray:
                {
                    return SBTType.Short;
                }

                case SBTType.UShortArray:
                {
                    return SBTType.UShort;
                }

                case SBTType.IntArray:
                {
                    return SBTType.Int;
                }

                case SBTType.UIntArray:
                {
                    return SBTType.UInt;
                }

                case SBTType.LongArray:
                {
                    return SBTType.Long;
                }

                case SBTType.ULongArray:
                {
                    return SBTType.ULong;
                }

                case SBTType.SingleArray:
                {
                    return SBTType.Single;
                }

                case SBTType.DoubleArray:
                {
                    return SBTType.Double;
                }

                default:
                {
                    throw new InvalidDataException("Type was not Array: " + type);
                }
            }
        }

        public static SBTType GetArrayType(this SBTType type)
        {
            switch (type)
            {
                case SBTType.String:
                {
                    return SBTType.StringArray;
                }
                    
                case SBTType.Byte:
                {
                    return SBTType.ByteArray;
                }

                case SBTType.Short:
                {
                    return SBTType.ShortArray;
                }

                case SBTType.UShort:
                {
                    return SBTType.UShortArray;
                }

                case SBTType.Int:
                {
                    return SBTType.IntArray;
                }

                case SBTType.UInt:
                {
                    return SBTType.UIntArray;
                }

                case SBTType.Long:
                {
                    return SBTType.LongArray;
                }

                case SBTType.ULong:
                {
                    return SBTType.ULongArray;
                }

                case SBTType.Single:
                {
                    return SBTType.SingleArray;
                }

                case SBTType.Double:
                {
                    return SBTType.DoubleArray;
                }

                default:
                {
                    throw new InvalidDataException("Type was not valid for Array: " + type);
                }
            }
        }
    }
}