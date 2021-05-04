namespace Craiel.UnityEssentialsUI.Runtime.GameControllers
{
    using JetBrains.Annotations;
    using UnityEngine;
    using UnityEngine.UI;

    public abstract class UIControllerHideable : UIControllerBase
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SerializeField]
        public Button ShowButton;

        [SerializeField]
        public bool HiddenByDefault;

        [UsedImplicitly]
        public override void Awake()
        {
            base.Awake();
            
            if (this.HiddenByDefault)
            {
                this.Hide();
            }
        }

        [UsedImplicitly]
        public virtual void OnHide()
        {
            this.Hide();
        }

        [UsedImplicitly]
        public virtual void OnShow()
        {
            this.Show();
        }

        public override void Hide()
        {
            base.Hide();
            this.ShowButton.gameObject.SetActive(this.IsHidden);
        }

        public override void Show()
        {
            base.Show();
            this.ShowButton.gameObject.SetActive(this.IsHidden);
        }
    }
}