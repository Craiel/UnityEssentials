namespace UnityGameDataExample.Runtime.Events
{
    using Craiel.UnityEssentials.Runtime.Contracts;
    using Enums;

    public class EventSwitchGameState : IGameEvent
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public EventSwitchGameState(GameMasterState master)
        {
            this.MasterState = master;
        }
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public GameMasterState MasterState { get; private set; }
    }
}