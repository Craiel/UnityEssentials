namespace Craiel.UnityEssentialsUI.Runtime.GameControllers
{
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class UIControllerDraggable : UIControllerBase, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool IsBeingDragged { get; private set; }
        
        public virtual void OnDrag(PointerEventData eventData)
        {
            this.transform.position = Input.mousePosition;
        }

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            this.IsBeingDragged = true;
        }
        
        public virtual void OnEndDrag(PointerEventData eventData)
        {
            this.IsBeingDragged = false;
        }
    }
}