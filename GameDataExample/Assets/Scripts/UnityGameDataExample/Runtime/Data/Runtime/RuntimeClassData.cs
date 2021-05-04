namespace UnityGameDataExample.Runtime.Data.Runtime
{
    using Craiel.UnityGameData.Runtime;
    using Enums;
    using UnityEngine;

    public class RuntimeClassData : RuntimeGameData
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SerializeField]
        public CharacterClassType InternalType;

        [SerializeField]
        public bool AvailableInCharacterCreation;

        [SerializeField]
        public GameDataId[] Factions;

        [SerializeField]
        public GameDataId[] Races;
    }
}