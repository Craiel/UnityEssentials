using Craiel.UnityEssentials.Runtime.Contracts;

namespace Craiel.UnityEssentials.Editor
{
    public class EssentialsDefaultConfig : IEssentialEditorConfig
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Configure()
        {
            SceneToolbar.RegisterWidget<SceneToolbarUtils>();
            SceneToolbar.RegisterWidget<SceneToolbarExtras>();
            SceneToolbar.RegisterWidget<SceneToolbarBuildGeneration>();
        }
    }
}