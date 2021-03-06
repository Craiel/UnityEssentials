﻿using HashUtils = Craiel.UnityEssentials.Runtime.Utils.HashUtils;

namespace Craiel.UnityEssentials.Runtime.Resource
{
    using System;
    using UnityEngine;

    public struct ResourceKey
    {
        public static readonly ResourceKey Invalid = new ResourceKey();

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ResourceKey(BundleKey bundle, string path, Type type)
            : this(path, type)
        {
            this.Bundle = bundle;
        }

        public ResourceKey(string path, Type type)
            : this()
        {
            this.Path = path;
            this.Type = type ?? TypeCache<UnityEngine.Object>.Value;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public BundleKey? Bundle { get; set; }

        public string Path { get; set; }

        public Type Type { get; set; }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(this.Path);
        }

        public static ResourceKey Create<T>(BundleKey bundle, string path)
        {
            return new ResourceKey(bundle, path, TypeCache<T>.Value);
        }

        public static ResourceKey Create<T>(string path)
        {
            return new ResourceKey(path, TypeCache<T>.Value);
        }

        public static bool operator ==(ResourceKey rhs, ResourceKey lhs)
        {
            return rhs.Bundle == lhs.Bundle
                && rhs.Path == lhs.Path
                && rhs.Type == lhs.Type;
        }

        public static bool operator !=(ResourceKey rhs, ResourceKey lhs)
        {
            return !(rhs == lhs);
        }

        public static ResourceKey GetFromString(string data)
        {
            return JsonUtility.FromJson<ResourceKey>(data);
        }

        public override int GetHashCode()
        {
            return HashUtils.GetSimpleCombinedHashCode(this.Bundle, this.Path);
        }

        public override bool Equals(object other)
        {
            if (other == null || other.GetType() != TypeCache<ResourceKey>.Value)
            {
                return false;
            }

            ResourceKey typed = (ResourceKey)other;
            return this == typed;
        }

        public override string ToString()
        {
            return string.Format("{0}:>{1} ({2})", this.Bundle == null ? "default" : this.Bundle.Value.ToString(), this.Path, this.Type);
        }

        public string GetAsString()
        {
            return JsonUtility.ToJson(this);
        }
    }
}
