namespace UnityGameDataExample.Editor.Data
{
    using Craiel.UnityGameData.Editor.Builder;
    using Craiel.UnityGameData.Editor.Common;
    using Runtime.Data.Runtime;
    using Runtime.Enums;
    using UnityEngine;

    public class GameDataFaction : GameDataObject
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SerializeField]
        public FactionType InternalType;
        
        [SerializeField]
        public bool AvailableInCharacterCreation;

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void DoBuild(GameDataBuildContext context)
        {
            base.DoBuild(context);
            
            var runtime = new RuntimeFactionData
            {
                InternalType = this.InternalType,
                AvailableInCharacterCreation = this.AvailableInCharacterCreation
            };

            this.BuildBase(context, runtime);

            context.AddBuildResult(runtime);
        }
    }
}