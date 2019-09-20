namespace Craiel.UnityEssentials.Runtime
{
    using System;

    public static class TypeCache<T>
    {
        public static readonly Type Value = typeof(T);
    }
}