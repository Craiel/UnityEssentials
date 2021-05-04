namespace Craiel.UnityAudio.Runtime.UI
{
    using Enums;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class UIAudioButtonEvents : MonoBehaviour, IPointerEnterHandler, ISelectHandler
    {
        private static readonly AudioPlayParameters PlayParameters = new AudioPlayParameters { UseRandomClip = true };

        private Button target;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SerializeField]
        public bool EnableClick = true;

        [SerializeField]
        public bool EnableHover = true;

        [SerializeField]
        public bool EnableSelect = true;

        [SerializeField]
        public bool IsPositiveClick = true;

        public void Awake()
        {
            this.target = this.GetComponent<Button>();
        }

        public void OnEnable()
        {
            if (this.target == null)
            {
                return;
            }
            
            this.target.onClick.AddListener(this.OnClick);
        }

        public void OnDisable()
        {
            if (this.target == null)
            {
                return;
            }

            this.target.onClick.RemoveListener(this.OnClick);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!this.EnableHover 
                || this.target == null 
                || !this.target.isActiveAndEnabled 
                || !this.target.interactable)
            {
                return;
            }

            AudioSystem.Instance.PlayAudioEventManaged(AudioEvent.ButtonHover, PlayParameters);
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (!this.EnableSelect 
                || this.target == null 
                || !this.target.isActiveAndEnabled 
                || !this.target.interactable)
            {
                return;
            }
            
            AudioSystem.Instance.PlayAudioEventManaged(AudioEvent.ButtonSelect, PlayParameters);
        }
        
        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void OnClick()
        {
            if (!this.EnableClick)
            {
                return;
            }

            if (this.IsPositiveClick)
            {
                AudioSystem.Instance.PlayAudioEventManaged(AudioEvent.ButtonClickPositive, PlayParameters);
            }
            else
            {
                AudioSystem.Instance.PlayAudioEventManaged(AudioEvent.ButtonClickNegative, PlayParameters);
            }
        }
    }
}