namespace Craiel.UnityEssentials.Runtime.Enums
{
    using System;

    [Flags]
    public enum SBTFlags
    {
        None = 0,
        
        
        // Reserved for debug / unit test
        Debug = 1 << 15,
        
        // Max value for this flag enum
        Reserved = 1 << 16
    }
}