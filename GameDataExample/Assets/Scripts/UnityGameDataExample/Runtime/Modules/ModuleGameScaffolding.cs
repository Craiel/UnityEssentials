namespace UnityGameDataExample.Runtime.Modules
{
    using Core;
    using Craiel.UnityEssentials.Runtime.EngineCore;
    using Craiel.UnityEssentials.Runtime.Event;
    using Enums;
    using Events;

    public class ModuleGameScaffolding : GameModuleBase<GameModules>
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ModuleGameScaffolding(GameModules parent)
            : base(parent)
        {
        }

        // ------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void Initialize()
        {
            base.Initialize();
            
            // General functions
            this.SubscribeEvent<EventStartNewGame>(this.OnStartNewGameRequest);
            this.SubscribeEvent<EventSwitchGameState>(this.OnGameStateSwitchRequest);
            this.SubscribeEvent<EventQuitGame>(this.OnQuitGameRequest);
            this.SubscribeEvent<EventSwitchGameMode>(this.OnSwitchGameMode);
            this.SubscribeEvent<EventModulesInitialized>(this.OnModulesInitialized);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void OnGameStateSwitchRequest(EventSwitchGameState eventData)
        {
            // First we ensure that we are in the right scene for the new master state
            EnsureTransitionForMasterState(eventData.MasterState);
            
            // Now set this as the active state
            this.Parent.GameState.MasterState = eventData.MasterState;
        }

        private static void EnsureTransitionForMasterState(GameMasterState masterState)
        {
            switch (masterState)
            {
                case GameMasterState.Intro:
                {
                    GameCore.Instance.Transition(GameSceneType.Intro);
                    break;
                }

                case GameMasterState.MainMenu:
                {
                    GameCore.Instance.Transition(GameSceneType.MainMenu);
                    break;
                }
                
                case GameMasterState.GameIntro:
                {
                    GameCore.Instance.Transition(GameSceneType.Game);
                    break;
                }
            }
        }
        
        private void OnQuitGameRequest(EventQuitGame eventdata)
        {
            // TODO: save game properly
            
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            UnityEngine.Application.Quit();
#endif
        }

        private void OnStartNewGameRequest(EventStartNewGame eventData)
        {
            // Prepare the save slot etc
            
            GameEvents.Send(new EventSwitchGameState(GameMasterState.GameIntro));
        }
        
        private void OnSwitchGameMode(EventSwitchGameMode eventData)
        {
            if (GameModules.Instance.GameState.Mode == eventData.Mode)
            {
                return;
            }
            
            GameMode previousMode = GameModules.Instance.GameState.Mode;
            GameModules.Instance.GameState.Mode = eventData.Mode;
            
            GameEvents.Send(new EventGameModeChanged(previousMode, eventData.Mode));
        }
        
        private void OnModulesInitialized(EventModulesInitialized eventData)
        {
            // This is the main entry point into the game runtime
            
            // Send the event to bring us to the new game state
            GameEvents.Send(new EventSwitchGameState(GameMasterState.Intro));
        }
    }
}