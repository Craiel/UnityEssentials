namespace Craiel.UnityEssentialsUI.Runtime.GameControllers
{
    using Enums;
    using Runtime;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class UIControllerBase : UIEngineBehavior, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SerializeField] 
        public GameObject Root;

        [SerializeField] 
        public GameObject NotifyRoot;

        [SerializeField] 
        public GameObject PointerMarkerRoot;

        [SerializeField]
        public GameObject FirstActiveControl;

        [SerializeField] 
        public UIControlAwakeState AwakeState;

        public bool IsHidden
        {
            get { return this.Root != null && !this.Root.activeSelf; }
        }

        public override void Awake()
        {
            base.Awake();
            
            if (this.Root == null)
            {
                EssentialCoreUI.Logger.Warn("UI Controller has no root set: {0} ({1})", this.name, this.GetType().Name);
            }

            switch (this.AwakeState)
            {
                case UIControlAwakeState.Hidden:
                {
                    this.Hide();
                    break;
                }

                case UIControlAwakeState.Visible:
                {
                    this.Show();
                    break;
                }
            }
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if (this.PointerMarkerRoot != null)
            {
                this.PointerMarkerRoot.SetActive(true);
            }
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if (this.PointerMarkerRoot != null)
            {
                this.PointerMarkerRoot.SetActive(false);
            }
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (this.NotifyRoot != null)
            {
                this.NotifyRoot.SetActive(false);
            }
        }
        
        public virtual void Hide()
        {
            this.ToggleRoot(false);
        }

        public virtual void Show()
        {
            this.ToggleRoot(true);

            if (this.FirstActiveControl != null && EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(this.FirstActiveControl);
            }
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected virtual void Notify()
        {
            if (this.NotifyRoot != null)
            {
                this.NotifyRoot.SetActive(true);
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void ToggleRoot(bool isVisible)
        {
            if (this.Root == null)
            {
                return;
            }

            if (isVisible && this.Root.activeSelf)
            {
                return;
            }

            if (!isVisible && !this.Root.activeSelf)
            {
                return;
            }

            this.Root.SetActive(isVisible);

#if UNITY_EDITOR
            EssentialCoreUI.Logger.Info("UICtrlToggle: {0} -> {1}", this.Root.name, isVisible);
#endif
        }
    }
}