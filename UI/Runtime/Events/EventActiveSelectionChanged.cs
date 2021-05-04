namespace Craiel.UnityEssentialsUI.Runtime.Events
{
    using UnityEngine;
    using UnityEssentials.Runtime.Contracts;

    public class EventActiveSelectionChanged : IGameEvent
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public EventActiveSelectionChanged(GameObject previous, GameObject current)
        {
            this.Previous = previous;
            this.Current = current;
        }
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public GameObject Previous { get; private set; }
        
        public GameObject Current { get; private set; }
    }
}