using Craiel.UnityAudio.Runtime;

namespace UnityGameDataExample.Runtime.Core
{
    using Craiel.UnityEssentials.Runtime.EngineCore;
    using Craiel.UnityGameData.Runtime;
    using Enums;
    using Scenes;
    using Setup;

    public class GameCore : EssentialEngineCore<GameCore, GameSceneType>
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public GameCore()
        {
            this.RegisterSceneImplementation<SceneIntro>(GameSceneType.Intro);
            this.RegisterSceneImplementation<SceneMainMenu>(GameSceneType.MainMenu);
            this.RegisterSceneImplementation<SceneGame>(GameSceneType.Game);
        }
        
        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void InitializeGameComponents()
        {
            GameRuntimeData.InstantiateAndInitialize();
            
            AudioSystem.InstantiateAndInitialize();
            
            GameModules.InstantiateAndInitialize();
        }
    }
}
