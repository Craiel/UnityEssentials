namespace Craiel.UnityEssentials.Enums
{
    using System;

    [Flags]
    public enum InputMappingState
    {
        None = 0,
        Held = 1 << 0,
        Down = 1 << 1,
        Up = 1 << 2
    }
}
