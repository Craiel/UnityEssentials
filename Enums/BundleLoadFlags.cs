﻿namespace Craiel.UnityEssentials.Enums
{
    using System;

    [Flags]
    public enum BundleLoadFlags
    {
        None = 0,
        Uncompressed = 1 << 0
    }
}
