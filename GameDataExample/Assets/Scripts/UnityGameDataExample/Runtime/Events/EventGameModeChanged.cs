namespace UnityGameDataExample.Runtime.Events
{
    using Craiel.UnityEssentials.Runtime.Contracts;
    using Enums;

    public class EventGameModeChanged : IGameEvent
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public EventGameModeChanged(GameMode previous, GameMode current)
        {
            this.Previous = previous;
            this.Current = current;
        }
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public GameMode Previous { get; private set; }
        
        public GameMode Current { get; private set; }
    }
}