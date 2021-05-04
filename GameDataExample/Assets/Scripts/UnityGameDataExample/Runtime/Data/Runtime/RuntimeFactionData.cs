namespace UnityGameDataExample.Runtime.Data.Runtime
{
    using Craiel.UnityGameData.Runtime;
    using Enums;
    using UnityEngine;

    public class RuntimeFactionData : RuntimeGameData
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SerializeField]
        public FactionType InternalType;

        [SerializeField]
        public bool AvailableInCharacterCreation;
    }
}