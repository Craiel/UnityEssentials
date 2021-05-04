namespace UnityGameDataExample.Runtime.Events
{
    using Craiel.UnityEssentials.Runtime.Contracts;
    using Enums;

    public class EventSwitchGameMode : IGameEvent
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public EventSwitchGameMode(GameMode mode)
        {
            this.Mode = mode;
        }
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public GameMode Mode { get; private set; }
    }
}