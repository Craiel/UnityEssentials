namespace UnityGameDataExample.Editor.Data
{
    using Craiel.UnityGameData.Editor.Builder;
    using Craiel.UnityGameData.Editor.Common;
    using Runtime.Data.Runtime;
    using Runtime.Enums;
    using UnityEngine;

    public class GameDataClass : GameDataObject
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SerializeField]
        public CharacterClassType InternalType;
        
        [SerializeField]
        public bool AvailableInCharacterCreation;

        [SerializeField]
        public GameDataFactionRef[] Factions;

        [SerializeField]
        public GameDataRaceRef[] Races;

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void DoBuild(GameDataBuildContext context)
        {
            base.DoBuild(context);
            
            var runtime = new RuntimeClassData
            {
                InternalType = this.InternalType,
                AvailableInCharacterCreation = this.AvailableInCharacterCreation,
                Factions = context.BuildGameDataIds(this, this.Factions),
                Races = context.BuildGameDataIds(this, this.Races)
            };

            this.BuildBase(context, runtime);

            context.AddBuildResult(runtime);
        }
    }
}