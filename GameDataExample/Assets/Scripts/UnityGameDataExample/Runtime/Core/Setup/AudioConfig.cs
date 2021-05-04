namespace UnityGameDataExample.Runtime.Core.Setup
{
    using Craiel.UnityAudio.Runtime;
    using Craiel.UnityAudio.Runtime.Contracts;
    using Craiel.UnityAudio.Runtime.Data;
    using Craiel.UnityEssentials.Runtime.Resource;
    using Craiel.UnityGameData.Runtime;
    using JetBrains.Annotations;
    using UnityEngine;
    using UnityEngine.Audio;

    [UsedImplicitly]
    public class AudioConfig : IAudioConfig
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Configure()
        {
            AudioCore.DynamicAudioSourceResource = ResourceKey.Create<GameObject>("Audio/DynamicAudioSource");
            AudioCore.MasterMixerResource = ResourceKey.Create<AudioMixer>("Audio/MasterMixer");
            
            GameRuntimeData.RegisterData<RuntimeAudioData>();
        }
    }
}