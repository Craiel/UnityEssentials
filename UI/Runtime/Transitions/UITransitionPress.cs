namespace Craiel.UnityEssentialsUI.Runtime.Transitions
{
    using System;
    using Enums;
    using TMPro;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    using UnityEssentials.Runtime.Utils;

    [AddComponentMenu(EssentialCoreUI.ComponentMenuFolder + "/Press Transition")]
    public class UITransitionPress : UITransitionBase
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void OnEnable()
        {
            this.InternalEvaluateAndTransitionToNormalState(true);
        }

        public void OnDisable()
        {
            this.InstantClearState();
        }
        
        public override void OnPointerDown(PointerEventData eventData)
        {
            this.DoStateTransition(UITransitionVisualState.Pressed, false);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            this.DoStateTransition(UITransitionVisualState.Normal, false);
        }
        
        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void InternalEvaluateAndTransitionToNormalState(bool instant)
        {
            this.DoStateTransition(UITransitionVisualState.Normal, instant);
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
                    this.SetTextColor(Color.white);
                    break;
                }

                case UITransitionMode.TextColorTMP:
                {
                    this.SetTextColorTMP(Color.white);
                    break;
                }
            }
        }
        
        private void DoStateTransition(UITransitionVisualState state, bool instant)
        {
            // Check if the script is enabled
            if (!this.gameObject.activeInHierarchy)
            {
                return;
            }

            // Check if it's interactable
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

                case UITransitionVisualState.Pressed:
                {
                    color = this.PressedColor;
                    newSprite = this.PressedSprite;
                    triggerName = this.PressedTrigger;
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
            if (this.TargetGameObject == null)
            {
                return;
            }

            UnityEngine.Animator animator = this.GetAnimator();
            
            if (animator == null 
                || !animator.enabled 
                || !animator.isActiveAndEnabled 
                || animator.runtimeAnimatorController == null 
                || !animator.hasBoundPlayables 
                || string.IsNullOrEmpty(triggerName))
            {
                return;
            }

            animator.ResetTrigger(this.PressedTrigger);
            animator.SetTrigger(triggerName);
        }
    }
}