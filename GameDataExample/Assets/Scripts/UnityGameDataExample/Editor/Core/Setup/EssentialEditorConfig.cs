namespace UnityGameDataExample.Core.Setup.Editor
{
    using Craiel.UnityEssentials.Editor;
    using Craiel.UnityEssentials.Runtime.Contracts;
    using Craiel.UnityGameData.Editor;
    using Craiel.UnityVFX.Editor;

    public class EssentialEditorConfig : IEssentialEditorConfig
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Configure()
        {
            SceneToolbar.ClearWidgets();
            
            SceneToolbar.RegisterWidget<SceneToolbarGameData>();
            SceneToolbar.RegisterWidget<SceneToolbarVFX>();
            SceneToolbar.RegisterWidget<SceneToolbarUtils>();
            SceneToolbar.RegisterWidget<SceneToolbarExtras>();
            SceneToolbar.RegisterWidget<SceneToolbarBuildGeneration>();
        }
    }
}