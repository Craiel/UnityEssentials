namespace Craiel.UnityEssentialsUI.Runtime.Transitions
{
    using Enums;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    [AddComponentMenu(EssentialCoreUI.ComponentMenuFolder + "/Effect Transition")]
    public class UITransitionEffect : UITransitionBase
    {
        private bool highlighted;
        private bool selected;
        private bool pressed;
        private bool active;
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SerializeField]
        [Tooltip("Graphic that will have the selected transition applied.")]
        public BaseMeshEffect TargetEffect;

        public override void Awake()
        {
            base.Awake();
            
            if (this.UseToggle)
            {
                if (this.TargetToggle == null)
                {
                    this.TargetToggle = this.gameObject.GetComponent<Toggle>();
                }

                if (this.TargetToggle != null)
                {
                    this.active = this.TargetToggle.isOn;
                }
            }
        }

        public void OnEnable()
        {
            if (this.TargetToggle != null)
            {
                this.TargetToggle.onValueChanged.AddListener(OnToggleValueChange);
            }

            this.InternalEvaluateAndTransitionToNormalState(true);
        }

        public void OnDisable()
        {
            if (this.TargetToggle != null)
            {
                this.TargetToggle.onValueChanged.RemoveListener(OnToggleValueChange);
            }

            this.InstantClearState();
        }

        public override void OnSelect(BaseEventData eventData)
        {
            this.selected = true;

            if (this.active)
            {
                return;
            }

            this.DoStateTransition(UITransitionVisualState.Selected, false);
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            this.selected = false;

            if (this.active)
            {
                return;
            }

            this.DoStateTransition((this.highlighted ? UITransitionVisualState.Highlighted : UITransitionVisualState.Normal), false);
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            this.highlighted = true;

            if (!this.selected && !this.pressed && !this.active)
            {
                this.DoStateTransition(UITransitionVisualState.Highlighted, false);
            }
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            this.highlighted = false;

            if (!this.selected && !this.pressed && !this.active)
            {
                this.DoStateTransition(UITransitionVisualState.Normal, false);
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            if (!this.highlighted)
            {
                return;
            }

            this.pressed = true;
            this.DoStateTransition(UITransitionVisualState.Pressed, false);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            this.pressed = false;

            UITransitionVisualState newState = UITransitionVisualState.Normal;

            if (this.active)
            {
                newState = UITransitionVisualState.Active;
            }
            else if (this.selected)
            {
                newState = UITransitionVisualState.Selected;
            }
            else if (this.highlighted)
            {
                newState = UITransitionVisualState.Highlighted;
            }

            this.DoStateTransition(newState, false);
        }
        
        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void InternalEvaluateAndTransitionToNormalState(bool instant)
        {
            this.DoStateTransition(this.active ? UITransitionVisualState.Active : UITransitionVisualState.Normal, instant);
        }
        
        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void OnToggleValueChange(bool value)
        {
            if (!this.UseToggle || this.TargetToggle == null)
            {
                return;
            }

            this.active = this.TargetToggle.isOn;

            if (!this.TargetToggle.isOn)
            {
                this.DoStateTransition(this.selected ? UITransitionVisualState.Selected : UITransitionVisualState.Normal, false);
            }
        }

        private void InstantClearState()
        {
            this.SetEffectColor(Color.white);
        }

        private void DoStateTransition(UITransitionVisualState state, bool instant)
        {
            // Check if active in the scene
            if (!this.gameObject.activeInHierarchy)
            {
                return;
            }

            if (!this.IsInteractable())
            {
                state = UITransitionVisualState.Normal;
            }

            Color color = this.NormalColor;

            // Prepare the transition values
            switch (state)
            {
                case UITransitionVisualState.Normal:
                    color = this.NormalColor;
                    break;
                case UITransitionVisualState.Highlighted:
                    color = this.HighlightedColor;
                    break;
                case UITransitionVisualState.Selected:
                    color = this.SelectedColor;
                    break;
                case UITransitionVisualState.Pressed:
                    color = this.PressedColor;
                    break;
                case UITransitionVisualState.Active:
                    color = this.ActiveColor;
                    break;
            }

            this.StartColorTween(this.GetEffectColor(), color, false, this.SetEffectColor);
        }

		private void SetEffectColor(Color targetColor)
        {
            if (this.TargetEffect == null)
            {
                return;
            }
            
            if (this.TargetEffect is Shadow)
            {
                (this.TargetEffect as Shadow).effectColor = targetColor;
            }
            else if (this.TargetEffect is Outline)
            {
                (this.TargetEffect as Outline).effectColor = targetColor;
            }
        }

        private Color GetEffectColor()
        {
            if (this.TargetEffect == null)
            {
                return Color.white;
            }

            if (this.TargetEffect is Shadow)
            {
                return (this.TargetEffect as Shadow).effectColor;
            }
            
            if (this.TargetEffect is Outline)
            {
                return (this.TargetEffect as Outline).effectColor;
            }

            return Color.white;
        }
    }
}