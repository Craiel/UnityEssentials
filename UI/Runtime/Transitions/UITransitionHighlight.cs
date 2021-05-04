namespace Craiel.UnityEssentialsUI.Runtime.Transitions
{
	using System;
	using Enums;
	using TMPro;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;
	using UnityEssentials.Runtime.Utils;

	[AddComponentMenu(EssentialCoreUI.ComponentMenuFolder + "/Highlight Transition")]
	public class UITransitionHighlight : UITransitionBase
    {
	    private bool highlighted;
	    private bool selected;
	    private bool pressed;
	    private bool active;

	    // -------------------------------------------------------------------
	    // Public
	    // -------------------------------------------------------------------
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
		
        public void OnToggleValueChange(bool value)
        {
	        if (!this.UseToggle || this.TargetToggle == null)
	        {
		        return;
	        }
            
            this.active = this.TargetToggle.isOn;
            
            if (this.TransitionMode == UITransitionMode.Animation)
            {
	            UnityEngine.Animator animator = this.GetAnimator();
	            if (this.TargetGameObject == null || 
	                animator == null || 
	                !animator.isActiveAndEnabled ||
	                animator.runtimeAnimatorController == null || 
	                string.IsNullOrEmpty(this.ActiveBool))
	            {
		            return;
	            }

                animator.SetBool(this.ActiveBool, this.active);
            }
            
            this.DoStateTransition(this.active ? UITransitionVisualState.Active : 
                this.selected ? UITransitionVisualState.Selected : this.highlighted ? UITransitionVisualState.Highlighted : UITransitionVisualState.Normal, false);
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
		private void InstantClearState()
		{
			switch (this.TransitionMode)
			{
				case UITransitionMode.ColorTint:
				{
					this.StartColorTween(Color.white, true);
					break;
				}

				case UITransitionMode.SpriteSwap:
				{
					this.DoSpriteSwap(null);
					break;
				}

				case UITransitionMode.TextColor:
				{
					this.SetTextColor(this.NormalColor);
					break;
				}

				case UITransitionMode.TextColorTMP:
				{
					this.SetTextColorTMP(this.NormalColor);
					break;
				}
			}
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
			Sprite newSprite = null;
			string triggerName = this.NormalTrigger;
			
			// Prepare the transition values
			switch (state)
			{
				case UITransitionVisualState.Normal:
				{
					color = this.NormalColor;
					triggerName = this.NormalTrigger;
					break;
				}

				case UITransitionVisualState.Highlighted:
				{
					color = this.HighlightedColor;
					newSprite = this.HighlightedSprite;
					triggerName = this.HighlightedTrigger;
					break;
				}

				case UITransitionVisualState.Selected:
				{
					color = this.SelectedColor;
					newSprite = this.SelectedSprite;
					triggerName = this.SelectedTrigger;
					break;
				}

				case UITransitionVisualState.Pressed:
				{
					color = this.PressedColor;
					newSprite = this.PressedSprite;
					triggerName = this.PressedTrigger;
					break;
				}

				case UITransitionVisualState.Active:
				{
					color = this.ActiveColor;
					newSprite = this.ActiveSprite;
					triggerName = this.HighlightedTrigger;
					break;
				}
			}
            
            // Do the transition
            switch (this.TransitionMode)
			{
				case UITransitionMode.ColorTint:
				{
					this.StartColorTween(color * this.ColorMultiplier, instant);
					break;
				}

				case UITransitionMode.SpriteSwap:
				{
					this.DoSpriteSwap(newSprite);
					break;
				}

				case UITransitionMode.Animation:
				{
					this.TriggerAnimation(triggerName);
					break;
				}

				case UITransitionMode.TextColor:
				{
					this.StartColorTween((this.TargetGraphic as Text).color, color, false, this.SetTextColor);
					break;
				}

				case UITransitionMode.TextColorTMP:
				{
					this.StartColorTween((this.TargetGraphic as TextMeshProUGUI).color, color, false, this.SetTextColorTMP);
					break;
				}
			}
		}
		
		private void StartColorTween(Color targetColor, bool instant)
		{
			if (this.TargetGraphic == null)
			{
				return;
			}
			
			if (instant || Math.Abs(this.Duration) < EssentialMathUtils.Epsilon || !Application.isPlaying)
			{
				this.TargetGraphic.canvasRenderer.SetColor(targetColor);
			}
			else
			{
				this.TargetGraphic.CrossFadeColor(targetColor, this.Duration, true, true);
			}
		}
		
		private void TriggerAnimation(string triggerName)
		{
			UnityEngine.Animator animator = this.GetAnimator();
			
			if (this.TargetGameObject == null || 
			    animator == null || 
			    !animator.isActiveAndEnabled ||
			    animator.runtimeAnimatorController == null || 
			    !animator.hasBoundPlayables ||
			    string.IsNullOrEmpty(triggerName))
			{
				return;
			}
            
            animator.ResetTrigger(this.HighlightedTrigger);
			animator.ResetTrigger(this.SelectedTrigger);
            animator.ResetTrigger(this.PressedTrigger);
            animator.SetTrigger(triggerName);
		}
    }
}