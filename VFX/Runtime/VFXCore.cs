namespace Craiel.UnityVFX.Runtime
{
    using Contracts;
    using UnityEssentials.Runtime.Component;

    public class VFXCore
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        static VFXCore()
        {
            new CraielComponentConfigurator<IVFXConfig>().Configure();
        }
    }
}