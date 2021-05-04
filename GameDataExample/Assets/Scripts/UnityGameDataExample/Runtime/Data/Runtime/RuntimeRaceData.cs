namespace UnityGameDataExample.Runtime.Data.Runtime
{
    using Craiel.UnityGameData.Runtime;
    using Enums;
    using UnityEngine;

    public class RuntimeRaceData : RuntimeGameData
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SerializeField]
        public CharacterRaceType InternalType;

        [SerializeField]
        public bool AvailableInCharacterCreation;

        [SerializeField]
        public GameDataId[] Factions;
    }
}