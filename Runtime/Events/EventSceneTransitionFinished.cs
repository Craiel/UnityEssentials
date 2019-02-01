namespace Craiel.UnityEssentialsUI.Runtime.Events
{
    using UnityEssentials.Runtime.Contracts;

    public class EventSceneTransitionFinished : IGameEvent
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public EventSceneTransitionFinished(object type)
        {
            this.Type = type;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public object Type { get; private set; }
    }
}