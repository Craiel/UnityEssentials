namespace Craiel.UnityEssentials.Runtime.Events
{
    using Contracts;

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