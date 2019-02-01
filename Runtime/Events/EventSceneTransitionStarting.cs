namespace Craiel.UnityEssentialsUI.Runtime.Events
{
    using UnityEssentials.Runtime.Contracts;

    public class EventSceneTransitionStarting : IGameEvent
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public EventSceneTransitionStarting(object type)
        {
            this.Type = type;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public object Type { get; private set; }
    }
}