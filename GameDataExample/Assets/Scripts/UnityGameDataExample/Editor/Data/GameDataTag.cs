namespace UnityGameDataExample.Data.Editor
{
    using Craiel.UnityGameData.Editor.Builder;
    using Craiel.UnityGameData.Editor.Common;
    using Runtime.Data.Runtime;
    using Runtime.Enums;
    using UnityEngine;

    public class GameDataTag : GameDataObject
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SerializeField] 
        public GameStateTagType Type;

        [SerializeField]
        public bool IsVisibleTag;

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void DoBuild(GameDataBuildContext context)
        {
            base.DoBuild(context);
            
            var runtime = new RuntimeTagData
            {
                Type = this.Type,
                IsVisibleTag = this.IsVisibleTag
            };

            this.BuildBase(context, runtime);

            context.AddBuildResult(runtime);
        }
    }
}