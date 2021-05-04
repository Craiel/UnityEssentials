namespace Craiel.UnityEssentialsUI.Runtime
{
    using Events;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEssentials.Runtime.Event;
    using UnityEssentials.Runtime.Singletons;

    public class UIEventSystemMonitor : UnitySingletonBehavior<UIEventSystemMonitor>
    {
        private GameObject lastSelectedObject;
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Update()
        {
            if (EventSystem.current == null)
            {
                return;
            }

            GameObject selectedObject = EventSystem.current.currentSelectedGameObject;
            if (selectedObject != this.lastSelectedObject)
            {
                GameEvents.Send(new EventActiveSelectionChanged(this.lastSelectedObject, selectedObject));
            }

            this.lastSelectedObject = selectedObject;
        }
    }
}