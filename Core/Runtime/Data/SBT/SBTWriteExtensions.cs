namespace Craiel.UnityEssentials.Runtime.Data.SBT
{
    using System;
    using Enums;
    using Nodes;

    public static class SBTWriteExtensions
    {
        public static SBTNodeList Add(this SBTNodeList target, string data, SBTFlags flags = SBTFlags.None, string note = null)
        {
            target.AddEntry(SBTType.String, data, flags, note);
            return target;
        }

        public static SBTNodeDictionary Add(this SBTNodeDictionary target, string key, string data, SBTFlags flags = SBTFlags.None, string note = null)
        {
            target.AddEntry(key, SBTType.String, data, flags, note);
            return target;
        }

        public static SBTNodeList Add(this SBTNodeList target, byte data, SBTFlags flags = SBTFlags.None, string note = null)
        {
            target.AddEntry(SBTType.Byte, data, flags, note);
            return target;
        }

        public static SBTNodeDictionary Add(this SBTNodeDictionary target, string key, byte data, SBTFlags flags = SBTFlags.None, string note = null)
        {
            target.AddEntry(key, SBTType.Byte, data, flags, note);
            return target;
        }

        public static SBTNodeList Add(this SBTNodeList target, short data, SBTFlags flags = SBTFlags.None, string note = null)
        {
            target.AddEntry(SBTType.Short, data, flags, note);
            return target;
        }

        public static SBTNodeDictionary Add(this SBTNodeDictionary target, string key, short data, SBTFlags flags = SBTFlags.None, string note = null)
        {
            target.AddEntry(key, SBTType.Short, data, flags, note);
            return target;
        }

        public static SBTNodeList Add(this SBTNodeList target, ushort data, SBTFlags flags = SBTFlags.None, string note = null)
        {
            target.AddEntry(SBTType.UShort, data, flags, note);
            return target;
        }

        public static SBTNodeDictionary Add(this SBTNodeDictionary target, string key, ushort data, SBTFlags flags = SBTFlags.None, string note = null)
        {
            target.AddEntry(key, SBTType.UShort, data, flags, note);
            return target;
        }

        public static SBTNodeList Add(this SBTNodeList target, int data, SBTFlags flags = SBTFlags.None, string note = null)
        {
            target.AddEntry(SBTType.Int, data, flags, note);
            return target;
        }

        public static SBTNodeDictionary Add(this SBTNodeDictionary target, string key, int data, SBTFlags flags = SBTFlags.None, string note = null)
        {
            target.AddEntry(key, SBTType.Int, data, flags, note);
            return target;
        }

        public static SBTNodeList Add(this SBTNodeList target, uint data, SBTFlags flags = SBTFlags.None, string note = null)
        {
            target.AddEntry(SBTType.UInt, data, flags, note);
            return target;
        }

        public static SBTNodeDictionary Add(this SBTNodeDictionary target, string key, uint data, SBTFlags flags = SBTFlags.None, string note = null)
        {
            target.AddEntry(key, SBTType.UInt, data, flags, note);
            return target;
        }

        public static SBTNodeList Add(this SBTNodeList target, long data, SBTFlags flags = SBTFlags.None, string note = null)
        {
            target.AddEntry(SBTType.Long, data, flags, note);
            return target;
        }

        public static SBTNodeDictionary Add(this SBTNodeDictionary target, string key, long data, SBTFlags flags = SBTFlags.None, string note = null)
        {
            target.AddEntry(key, SBTType.Long, data, flags, note);
            return target;
        }

        public static SBTNodeList Add(this SBTNodeList target, ulong data, SBTFlags flags = SBTFlags.None, string note = null)
        {
            target.AddEntry(SBTType.ULong, data, flags, note);
            return target;
        }

        public static SBTNodeDictionary Add(this SBTNodeDictionary target, string key, ulong data, SBTFlags flags = SBTFlags.None, string note = null)
        {
            target.AddEntry(key, SBTType.ULong, data, flags, note);
            return target;
        }

        public static SBTNodeList Add(this SBTNodeList target, float data, SBTFlags flags = SBTFlags.None, string note = null)
        {
            target.AddEntry(SBTType.Single, data, flags, note);
            return target;
        }

        public static SBTNodeDictionary Add(this SBTNodeDictionary target, string key, float data, SBTFlags flags = SBTFlags.None, string note = null)
        {
            target.AddEntry(key, SBTType.Single, data, flags, note);
            return target;
        }

        public static SBTNodeList Add(this SBTNodeList target, double data, SBTFlags flags = SBTFlags.None, string note = null)
        {
            target.AddEntry(SBTType.Double, data, flags, note);
            return target;
        }

        public static SBTNodeDictionary Add(this SBTNodeDictionary target, string key, double data, SBTFlags flags = SBTFlags.None, string note = null)
        {
            target.AddEntry(key, SBTType.Double, data, flags, note);
            return target;
        }

        public static SBTNodeList AddList(this SBTNodeList target, SBTFlags flags = SBTFlags.None, string note = null)
        {
            return (SBTNodeList)target.AddEntry(SBTType.List, null, flags, note);
        }

        public static SBTNodeList AddList(this SBTNodeDictionary target, string key, SBTFlags flags = SBTFlags.None, string note = null)
        {
            return (SBTNodeList)target.AddEntry(key, SBTType.List, null, flags, note);
        }

        public static SBTNodeDictionary AddDictionary(this SBTNodeList target, SBTFlags flags = SBTFlags.None, string note = null)
        {
            return (SBTNodeDictionary)target.AddEntry(SBTType.Dictionary, null, flags, note);
        }

        public static SBTNodeDictionary AddDictionary(this SBTNodeDictionary target, string key, SBTFlags flags = SBTFlags.None, string note = null)
        {
            return (SBTNodeDictionary)target.AddEntry(key, SBTType.Dictionary, null, flags, note);
        }

        public static SBTNodeArray<T> AddArray<T>(this SBTNodeList target, SBTFlags flags = SBTFlags.None, string note = null)
        {
            SBTType type = SBTUtils.GetArrayBaseType(TypeCache<T>.Value);
            return (SBTNodeArray<T>)target.AddEntry(type, null, flags, note);
        }

        public static void AddArray<T>(this SBTNodeList target, T[] values, SBTFlags flags = SBTFlags.None, string note = null)
        {
            SBTType type = SBTUtils.GetArrayBaseType(TypeCache<T>.Value);
            var array = (SBTNodeArray<T>) target.AddEntry(type, null, flags, note);
            array.Add(values);
        }

        public static SBTNodeArray<T> AddArray<T>(this SBTNodeDictionary target, string key, SBTFlags flags = SBTFlags.None, string note = null)
        {
            SBTType type = SBTUtils.GetArrayBaseType(TypeCache<T>.Value);
            return (SBTNodeArray<T>)target.AddEntry(key, type, null, flags, note);
        }

        public static void AddArray<T>(this SBTNodeDictionary target, string key, T[] values, SBTFlags flags = SBTFlags.None, string note = null)
        {
            SBTType type = SBTUtils.GetArrayBaseType(TypeCache<T>.Value);
            var array = (SBTNodeArray<T>) target.AddEntry(key, type, null, flags, note);
            array.SetCapacity(values.Length);
            array.Add(values);
        }

        public static SBTNodeStream AddStream(this SBTNodeList target, SBTFlags flags = SBTFlags.None, string note = null)
        {
            return (SBTNodeStream)target.AddEntry(SBTType.Stream, null, flags, note);
        }

        public static SBTNodeStream AddStream(this SBTNodeDictionary target, string key, SBTFlags flags = SBTFlags.None, string note = null)
        {
            return (SBTNodeStream)target.AddEntry(key, SBTType.Stream, null, flags, note);
        }

        public static SBTNodeList Add(this SBTNodeList target, DateTime data, SBTFlags flags = SBTFlags.None, string note = null)
        {
            target.AddEntry(SBTType.DateTime, data, flags, note);
            return target;
        }

        public static SBTNodeDictionary Add(this SBTNodeDictionary target, string key, DateTime data, SBTFlags flags = SBTFlags.None, string note = null)
        {
            target.AddEntry(key, SBTType.DateTime, data, flags, note);
            return target;
        }

        public static SBTNodeList Add(this SBTNodeList target, TimeSpan data, SBTFlags flags = SBTFlags.None, string note = null)
        {
            target.AddEntry(SBTType.TimeSpan, data, flags, note);
            return target;
        }

        public static SBTNodeDictionary Add(this SBTNodeDictionary target, string key, TimeSpan data, SBTFlags flags = SBTFlags.None, string note = null)
        {
            target.AddEntry(key, SBTType.TimeSpan, data, flags, note);
            return target;
        }
    }
}