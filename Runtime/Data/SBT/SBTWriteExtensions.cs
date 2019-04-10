namespace Craiel.UnityEssentials.Runtime.Data.SBT
{
    using System;
    using Enums;
    using Nodes;

    public static class SBTWriteExtensions
    {
        public static SBTNodeList Add(this SBTNodeList target, string data, SBTFlags flags = SBTFlags.None)
        {
            target.AddEntry(SBTType.String, data, flags);
            return target;
        }
        
        public static SBTNodeDictionary Add(this SBTNodeDictionary target, string key, string data, SBTFlags flags = SBTFlags.None)
        {
            target.AddEntry(key, SBTType.String, data, flags);
            return target;
        }
        
        public static SBTNodeList Add(this SBTNodeList target, byte data, SBTFlags flags = SBTFlags.None)
        {
            target.AddEntry(SBTType.Byte, data, flags);
            return target;
        }
        
        public static SBTNodeDictionary Add(this SBTNodeDictionary target, string key, byte data, SBTFlags flags = SBTFlags.None)
        {
            target.AddEntry(key, SBTType.Byte, data, flags);
            return target;
        }
        
        public static SBTNodeList Add(this SBTNodeList target, short data, SBTFlags flags = SBTFlags.None)
        {
            target.AddEntry(SBTType.Short, data, flags);
            return target;
        }
        
        public static SBTNodeDictionary Add(this SBTNodeDictionary target, string key, short data, SBTFlags flags = SBTFlags.None)
        {
            target.AddEntry(key, SBTType.Short, data, flags);
            return target;
        }
        
        public static SBTNodeList Add(this SBTNodeList target, ushort data, SBTFlags flags = SBTFlags.None)
        {
            target.AddEntry(SBTType.UShort, data, flags);
            return target;
        }
        
        public static SBTNodeDictionary Add(this SBTNodeDictionary target, string key, ushort data, SBTFlags flags = SBTFlags.None)
        {
            target.AddEntry(key, SBTType.UShort, data, flags);
            return target;
        }
        
        public static SBTNodeList Add(this SBTNodeList target, int data, SBTFlags flags = SBTFlags.None)
        {
            target.AddEntry(SBTType.Int, data, flags);
            return target;
        }
        
        public static SBTNodeDictionary Add(this SBTNodeDictionary target, string key, int data, SBTFlags flags = SBTFlags.None)
        {
            target.AddEntry(key, SBTType.Int, data, flags);
            return target;
        }
        
        public static SBTNodeList Add(this SBTNodeList target, uint data, SBTFlags flags = SBTFlags.None)
        {
            target.AddEntry(SBTType.UInt, data, flags);
            return target;
        }
        
        public static SBTNodeDictionary Add(this SBTNodeDictionary target, string key, uint data, SBTFlags flags = SBTFlags.None)
        {
            target.AddEntry(key, SBTType.UInt, data, flags);
            return target;
        }
        
        public static SBTNodeList Add(this SBTNodeList target, long data, SBTFlags flags = SBTFlags.None)
        {
            target.AddEntry(SBTType.Long, data, flags);
            return target;
        }
        
        public static SBTNodeDictionary Add(this SBTNodeDictionary target, string key, long data, SBTFlags flags = SBTFlags.None)
        {
            target.AddEntry(key, SBTType.Long, data, flags);
            return target;
        }
        
        public static SBTNodeList Add(this SBTNodeList target, ulong data, SBTFlags flags = SBTFlags.None)
        {
            target.AddEntry(SBTType.ULong, data, flags);
            return target;
        }
        
        public static SBTNodeDictionary Add(this SBTNodeDictionary target, string key, ulong data, SBTFlags flags = SBTFlags.None)
        {
            target.AddEntry(key, SBTType.ULong, data, flags);
            return target;
        }
        
        public static SBTNodeList Add(this SBTNodeList target, float data, SBTFlags flags = SBTFlags.None)
        {
            target.AddEntry(SBTType.Single, data, flags);
            return target;
        }
        
        public static SBTNodeDictionary Add(this SBTNodeDictionary target, string key, float data, SBTFlags flags = SBTFlags.None)
        {
            target.AddEntry(key, SBTType.Single, data, flags);
            return target;
        }
        
        public static SBTNodeList Add(this SBTNodeList target, double data, SBTFlags flags = SBTFlags.None)
        {
            target.AddEntry(SBTType.Double, data, flags);
            return target;
        }
        
        public static SBTNodeDictionary Add(this SBTNodeDictionary target, string key, double data)
        {
            target.AddEntry(key, SBTType.Double, data);
            return target;
        }

        public static SBTNodeList AddList(this SBTNodeList target)
        {
            return (SBTNodeList)target.AddEntry(SBTType.List);
        }
        
        public static SBTNodeList AddList(this SBTNodeDictionary target, string key)
        {
            return (SBTNodeList)target.AddEntry(key, SBTType.List);
        }
        
        public static SBTNodeDictionary AddDictionary(this SBTNodeList target)
        {
            return (SBTNodeDictionary)target.AddEntry(SBTType.Dictionary);
        }
        
        public static SBTNodeDictionary AddDictionary(this SBTNodeDictionary target, string key)
        {
            return (SBTNodeDictionary)target.AddEntry(key, SBTType.Dictionary);
        }
        
        public static SBTNodeArray<T> AddArray<T>(this SBTNodeList target)
        {
            SBTType type = SBTUtils.GetArrayBaseType(typeof(T));
            return (SBTNodeArray<T>)target.AddEntry(type);
        }
        
        public static SBTNodeArray<T> AddArray<T>(this SBTNodeDictionary target, string key)
        {
            SBTType type = SBTUtils.GetArrayBaseType(typeof(T));
            return (SBTNodeArray<T>)target.AddEntry(key, type);
        }
    }
}