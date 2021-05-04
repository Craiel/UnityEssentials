namespace UnityGameDataExample.Runtime.Core.Scenes
{
    using Craiel.UnityEssentials.Runtime.Scene;
    using Enums;

    public class SceneIntro : BaseScene<GameSceneType>
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SceneIntro() 
            : base("Intro", "Intro")
        {
        }
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override GameSceneType Type
        {
            get { return GameSceneType.Intro; }
        }
    }
}
