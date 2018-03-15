﻿namespace Craiel.UnityEssentials.Resource
{
    using Enums;

    public struct ResourceLoadInfo
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ResourceLoadInfo(ResourceKey key, ResourceLoadFlags flags)
            : this()
        {
            this.Key = key;
            this.Flags = flags;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public ResourceKey Key { get; private set; }

        public ResourceLoadFlags Flags { get; private set; }
    }
}
