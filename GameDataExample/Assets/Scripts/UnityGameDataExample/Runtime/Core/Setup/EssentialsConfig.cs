using Craiel.UnityAudio.Runtime.Data;

namespace UnityGameDataExample.Runtime.Core.Setup
{
    using Craiel.UnityEssentials.Runtime.Contracts;
    using Craiel.UnityGameData.Runtime;
    using Data.Runtime;
    using JetBrains.Annotations;

    [UsedImplicitly]
    public class EssentialsConfig : IEssentialConfig
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Configure()
        {
            GameRuntimeData.RegisterData<RuntimeAudioData>();
            GameRuntimeData.RegisterData<RuntimeRarityData>();
            GameRuntimeData.RegisterData<RuntimeTagData>();
            GameRuntimeData.RegisterData<RuntimeFactionData>();
            GameRuntimeData.RegisterData<RuntimeClassData>();
            GameRuntimeData.RegisterData<RuntimeRaceData>();
        }
    }
}
