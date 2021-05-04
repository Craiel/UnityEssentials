namespace Craiel.UnityAudio.Runtime
{
    using Contracts;
    using NLog;
    using UnityEssentials.Runtime.Component;
    using UnityEssentials.Runtime.Resource;

    public static class AudioCore
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        static AudioCore()
        {
            new CraielComponentConfigurator<IAudioConfig>().Configure();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static ResourceKey MasterMixerResource { get; set; }
        public static ResourceKey DynamicAudioSourceResource { get; set; }
        public static ResourceKey AudioEventMappingResource { get; set; }

        public static readonly Logger Logger = LogManager.GetLogger("CRAIEL_AUDIO");
    }
}
