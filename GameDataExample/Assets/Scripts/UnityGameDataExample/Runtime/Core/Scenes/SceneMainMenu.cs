namespace UnityGameDataExample.Runtime.Core.Scenes
{
    using Craiel.UnityEssentials.Runtime.Scene;
    using Enums;

    public class SceneMainMenu : BaseScene<GameSceneType>
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SceneMainMenu()
            : base("MainMenu", "MainMenu")
        {
            this.AdditiveLevelNames.Add("UI");
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override GameSceneType Type
        {
            get { return GameSceneType.MainMenu; }
        }
    }
}
