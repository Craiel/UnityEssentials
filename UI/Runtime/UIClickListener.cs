namespace Craiel.UnityEssentialsUI.Runtime
{
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;

    public class UIClickListener : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        public PointerEventData.InputButton Button;

        [SerializeField]
        public UnityEvent<PointerEventData> OnClick;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != this.Button)
            {
                return;
            }
            
            this.OnClick?.Invoke(eventData);
        }
    }
}