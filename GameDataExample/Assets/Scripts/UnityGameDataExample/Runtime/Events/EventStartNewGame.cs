namespace UnityGameDataExample.Runtime.Events
{
    using Craiel.UnityEssentials.Runtime.Contracts;

    public class EventStartNewGame : IGameEvent
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public EventStartNewGame(ushort saveSlot)
        {
            this.SaveSlot = saveSlot;
        }
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public ushort SaveSlot { get; private set; }
    }
}