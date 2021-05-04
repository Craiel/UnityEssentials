namespace UnityGameDataExample.Runtime.Core.Scenes
{
    using Craiel.UnityEssentials.Runtime.Event;
    using Craiel.UnityEssentials.Runtime.Scene;
    using Enums;
    using Events;

    public class SceneGame : BaseScene<GameSceneType>
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SceneGame() 
            : base("Game", "Game")
        {
        }
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override GameSceneType Type
        {
            get { return GameSceneType.Game; }
        }

        protected override bool ScenePostLoad()
        {
            // Send some events to get things started
            GameEvents.Send(new EventGameModeChanged(GameMode.None, GameModules.Instance.GameState.Mode));
            
            return base.ScenePostLoad();
        }
    }
}
