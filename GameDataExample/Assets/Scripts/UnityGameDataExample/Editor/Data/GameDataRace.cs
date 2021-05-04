namespace UnityGameDataExample.Editor.Data
{
    using Craiel.UnityGameData.Editor.Builder;
    using Craiel.UnityGameData.Editor.Common;
    using Runtime.Data;
    using Runtime.Data.Runtime;
    using Runtime.Enums;
    using UnityEngine;

    public class GameDataRace: GameDataObject
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SerializeField]
        public CharacterRaceType InternalType;
        
        [SerializeField]
        public bool AvailableInCharacterCreation;

        [SerializeField]
        public GameDataFactionRef[] Factions;

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void DoBuild(GameDataBuildContext context)
        {
            base.DoBuild(context);
            
            var runtime = new RuntimeRaceData
            {
                InternalType = this.InternalType,
                AvailableInCharacterCreation = this.AvailableInCharacterCreation,
                Factions = context.BuildGameDataIds(this, this.Factions)
            };

            this.BuildBase(context, runtime);

            context.AddBuildResult(runtime);
        }
    }
}