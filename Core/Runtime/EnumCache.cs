namespace Craiel.UnityEssentials.Runtime
{
    using System;

    public static class EnumCache<T>
        where T : struct, IConvertible
    {
        public static readonly Type Type;

        public static readonly int Count;

        public static readonly T[] Values;
        public static readonly string[] Names;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        static EnumCache()
        {
            Type = TypeCache<T>.Value;

            Values = (T[])Enum.GetValues(Type);
            Names = Enum.GetNames(Type);

            Count = Values.Length;
        }
    }
}