namespace Craiel.UnityEssentialsUI.Runtime.Transitions
{
    using System;
    using System.Collections.Generic;
    using Enums;
    using TMPro;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    using UnityEssentials.Runtime.TweenLite;
    using UnityEssentials.Runtime.Utils;

    public abstract class UITransitionBase : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        private readonly List<CanvasGroup> canvasGroupCache = new List<CanvasGroup>();
        
        private Selectable selectable;
        private bool groupsAllowInteraction = true;
        
        private TweenLiteTicket activeColorTweenTicket;
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SerializeField] 
        public UITransitionMode TransitionMode = UITransitionMode.None;
        
        [SerializeField] 
        public Color NormalColor = ColorBlock.defaultColorBlock.normalColor;
        
        [SerializeField] 
        public Color HighlightedColor = ColorBlock.defaultColorBlock.highlightedColor;
		
        [SerializeField] 
        public Color SelectedColor = ColorBlock.defaultColorBlock.highlightedColor;
		
        [SerializeField] 
        public Color PressedColor = ColorBlock.defaultColorBlock.pressedColor;
        
        [SerializeField] 
        public Color ActiveColor = ColorBlock.defaultColorBlock.highlightedColor;
        
        [SerializeField] 
        public float Duration = 0.1f;
		
        [SerializeField]
        [Range(1f, 6f)]
        public float ColorMultiplier = 1f;
        
        [SerializeField] 
        public Sprite HighlightedSprite;
		
        [SerializeField] 
        public Sprite SelectedSprite;
		
        [SerializeField] 
        public Sprite PressedSprite;
		
        [SerializeField] 
        public Sprite ActiveSprite;
        
        [SerializeField] 
        public string NormalTrigger = "Normal";
        
        [SerializeField] 
        public string HighlightedTrigger = "Highlighted";
		
        [SerializeField] 
        public string SelectedTrigger = "Selected";
		
        [SerializeField] 
        public string PressedTrigger = "Pressed";
        
        [SerializeField]
        [Tooltip("Graphic that will have the selected transition applied.")]
        public Graphic TargetGraphic;
		
        [SerializeField]
        [Tooltip("GameObject that will have the selected transition applied.")]
        public GameObject TargetGameObject;
        
        [SerializeField] 
        public bool UseToggle;
        
        [SerializeField] 
        public Toggle TargetToggle;
        
        [SerializeField] 
        public string ActiveBool = "Active";
        
        public virtual bool IsInteractable()
        {
            if (this.selectable != null)
            {
                return this.selectable.IsInteractable() && this.groupsAllowInteraction;
            }

            return this.groupsAllowInteraction;
        }
        
        public virtual void OnSelect(BaseEventData eventData)
        {
        }

        public virtual void OnDeselect(BaseEventData eventData)
        {
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
        }
        
        public virtual void Awake()
        {
            this.selectable = this.gameObject.GetComponent<Selectable>();
        }
        
        public void OnCanvasGroupChanged()
        {
            var groupAllowInteraction = true;
            Transform tempTransform = this.transform;
            while (tempTransform != null)
            {
                tempTransform.GetComponents(this.canvasGroupCache);
                bool shouldBreak = false;
                for (var i = 0; i < this.canvasGroupCache.Count; i++)
                {
                    // if the parent group does not allow interaction we need to break
                    if (!this.canvasGroupCache[i].interactable)
                    {
                        groupAllowInteraction = false;
                        shouldBreak = true;
                    }
                    // if this is a 'fresh' group, then break as we should not consider parents
                    if (this.canvasGroupCache[i].ignoreParentGroups)
                    {
                        shouldBreak = true;
                    }
                }

                if (shouldBreak)
                {
                    break;
                }

                tempTransform = tempTransform.parent;
            }

            if (groupAllowInteraction != this.groupsAllowInteraction)
            {
                this.groupsAllowInteraction = groupAllowInteraction;
                this.InternalEvaluateAndTransitionToNormalState(true);
            }
        }
        
#if UNITY_EDITOR
        public void OnValidate()
        {
            this.Duration = Mathf.Max(this.Duration, 0f);
			
            if (this.isActiveAndEnabled)
            {
                this.DoSpriteSwap(null);
                this.InternalEvaluateAndTransitionToNormalState(true);
            }
        }
#endif

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected abstract void InternalEvaluateAndTransitionToNormalState(bool instant);
        
        protected Animator GetAnimator()
        {
            if (this.TargetGameObject != null)
            {
                return this.TargetGameObject.GetComponent<Animator>();
            }
				
            return null;
        }
        
        protected void StartColorTween(Color currentColor, Color targetColor, bool instant, TweenLiteColorDelegate callback)
        {
            if (this.TargetGraphic == null)
            {
                return;
            }

            if (instant || Math.Abs(this.Duration) < EssentialMathUtils.Epsilon || !Application.isPlaying)
            {
                callback(targetColor);
            }
            else
            {
                if (this.activeColorTweenTicket != TweenLiteTicket.Invalid)
                {
                    TweenLiteSystem.Instance.StopTween(ref this.activeColorTweenTicket);
                }
	            
                var colorTween = new TweenLiteColor(currentColor, targetColor, callback)
                {
                    Duration = this.Duration,
                    IgnoreTimeScale = true
                };

                TweenLiteSystem.Instance.StartTween(colorTween);
                this.activeColorTweenTicket = colorTween.Ticket;
            }
        }
        
        protected void DoSpriteSwap(Sprite newSprite)
        {
            Image image = this.TargetGraphic as Image;

            if (image == null)
            {
                return;
            }

            image.overrideSprite = newSprite;
        }
        
        protected void SetTextColor(Color targetColor)
        {
            if (this.TargetGraphic == null)
            {
                return;
            }

            if (this.TargetGraphic is Text)
            {
                (this.TargetGraphic as Text).color = targetColor;
            }
        }

        protected void SetTextColorTMP(Color targetColor)
        {
            if (this.TargetGraphic == null)
            {
                return;
            }

            if (this.TargetGraphic is TextMeshProUGUI)
            {
                (this.TargetGraphic as TextMeshProUGUI).color = targetColor;
            }
        }
    }
}