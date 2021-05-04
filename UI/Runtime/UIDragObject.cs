namespace Craiel.UnityEssentialsUI.Runtime
{
    using Enums;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEssentials.Runtime.Extensions;

    public delegate void DragObjectDelegate(PointerEventData data);

    [AddComponentMenu(EssentialCoreUI.ComponentMenuFolder + "/Drag Object")]
    public class UIDragObject : UIBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private Canvas canvas;
        private RectTransform canvasRectTransform;
        private Vector2 pointerStartPosition = Vector2.zero;
        private Vector2 targetStartPosition = Vector2.zero;
        private Vector2 currentVelocity;
        private bool dragging;
        private Vector2 lastPosition = Vector2.zero;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public event DragObjectDelegate BeginDrag;
        public event DragObjectDelegate EndDrag;
        public event DragObjectDelegate Drag;

        [SerializeField] 
        public RectTransform Target;
        
        [SerializeField] 
        public bool Horizontal = true;
        
        [SerializeField] 
        public bool Vertical = true;
        
        [SerializeField] 
        public bool Inertia = true;
        
        [SerializeField] 
        public UIIntertiaRoundingMode InertiaRounding = UIIntertiaRoundingMode.Hard;
        
        [SerializeField] 
        public float DampeningRate = 9f;
        
        [SerializeField] 
        public bool ConstrainWithinCanvas;
        
        [SerializeField] 
        public bool ConstrainDrag = true;
        
        [SerializeField]
        public bool ConstrainInertia = true;
        
        public override bool IsActive()
        {
            return base.IsActive() && this.Target != null;
        }

        public void StopMovement()
        {
            this.currentVelocity = Vector2.zero;
        }

        public void OnBeginDrag(PointerEventData data)
        {
            if (!this.IsActive())
            {
                return;
            }

            RectTransformUtility.ScreenPointToLocalPointInRectangle(this.canvasRectTransform, data.position, data.pressEventCamera, out this.pointerStartPosition);
            this.targetStartPosition = this.Target.anchoredPosition;
            this.currentVelocity = Vector2.zero;
            this.dragging = true;

            this.BeginDrag?.Invoke(data);
        }

        public void OnEndDrag(PointerEventData data)
        {
            this.dragging = false;

            if (!this.IsActive())
            {
                return;
            }

            this.EndDrag?.Invoke(data);
        }

        public void OnDrag(PointerEventData data)
        {
            if (!this.IsActive() || this.canvas == null)
            {
                return;
            }

            Vector2 mousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(this.canvasRectTransform, data.position, data.pressEventCamera, out mousePos);

            if (this.ConstrainWithinCanvas && this.ConstrainDrag)
            {
                mousePos = this.ClampToCanvas(mousePos);
            }

            Vector2 newPosition = this.targetStartPosition + (mousePos - this.pointerStartPosition);

            // Restrict movement on the axis
            if (!this.Horizontal)
            {
                newPosition.x = this.Target.anchoredPosition.x;
            }
            if (!this.Vertical)
            {
                newPosition.y = this.Target.anchoredPosition.y;
            }

            // Apply the position change
            this.Target.anchoredPosition = newPosition;

            this.Drag?.Invoke(data);
        }
        
        public virtual void LateUpdate()
        {
            if (!this.Target)
            {
                return;
            }

            // Capture the velocity of our drag to be used for the inertia
            if (this.dragging && this.Inertia)
            {
                Vector3 to = (this.Target.anchoredPosition - this.lastPosition) / Time.unscaledDeltaTime;
                this.currentVelocity = Vector3.Lerp(this.currentVelocity, to, Time.unscaledDeltaTime * 10f);
            }

            this.lastPosition = this.Target.anchoredPosition;

            // Handle inertia only when not dragging
            if (!this.dragging && this.currentVelocity != Vector2.zero)
            {
                Vector2 anchoredPosition = this.Target.anchoredPosition;

                // Dampen the inertia
                this.Dampen(ref this.currentVelocity, this.DampeningRate, Time.unscaledDeltaTime);

                for (int i = 0; i < 2; i++)
                {
                    // Calculate the inertia amount to be applied on this update
                    if (this.Inertia)
                    {
                        anchoredPosition[i] += this.currentVelocity[i] * Time.unscaledDeltaTime;
                    }
                    else
                    {
                        this.currentVelocity[i] = 0f;
                    }
                }

                if (this.currentVelocity != Vector2.zero)
                {
                    // Restrict movement on the axis
                    if (!this.Horizontal)
                    {
                        anchoredPosition.x = this.Target.anchoredPosition.x;
                    }
                    if (!this.Vertical)
                    {
                        anchoredPosition.y = this.Target.anchoredPosition.y;
                    }

                    // If the target is constrained within it's canvas
                    if (this.ConstrainWithinCanvas && this.ConstrainInertia && this.canvasRectTransform != null)
                    {
                        Vector3[] canvasCorners = new Vector3[4];
                        this.canvasRectTransform.GetWorldCorners(canvasCorners);

                        Vector3[] targetCorners = new Vector3[4];
                        this.Target.GetWorldCorners(targetCorners);

                        // Outside of the screen to the left or right
                        if (targetCorners[0].x < canvasCorners[0].x || targetCorners[2].x > canvasCorners[2].x)
                        {
                            anchoredPosition.x = this.Target.anchoredPosition.x;
                        }

                        // Outside of the screen to the top or bottom
                        if (targetCorners[3].y < canvasCorners[3].y || targetCorners[1].y > canvasCorners[1].y)
                        {
                            anchoredPosition.y = this.Target.anchoredPosition.y;
                        }
                    }

                    // Apply the inertia
                    if (anchoredPosition != this.Target.anchoredPosition)
                    {
                        switch (this.InertiaRounding)
                        {
                            case UIIntertiaRoundingMode.Hard:
                            {
                                this.Target.anchoredPosition = new Vector2(Mathf.Round(anchoredPosition.x / 2f) * 2f, Mathf.Round(anchoredPosition.y / 2f) * 2f);
                                break;
                            }

                            default:
                            {
                                this.Target.anchoredPosition = new Vector2(Mathf.Round(anchoredPosition.x), Mathf.Round(anchoredPosition.y));
                                break;
                            }
                        }
                    }
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();
            this.canvas = (this.Target != null ? this.Target.gameObject : this.gameObject).FindInParents<Canvas>();
            if (this.canvas != null)
            {
                this.canvasRectTransform = this.canvas.transform as RectTransform;
            }
        }

        protected override void OnTransformParentChanged()
        {
            base.OnTransformParentChanged();
            this.canvas = (this.Target != null ? this.Target.gameObject : this.gameObject).FindInParents<Canvas>();
            if (this.canvas != null)
            {
                this.canvasRectTransform = this.canvas.transform as RectTransform;
            }
        }
        
        private Vector3 Dampen(ref Vector2 velocity, float strength, float delta)
        {
            if (delta > 1f)
            {
                delta = 1f;
            }

            float dampeningFactor = 1f - strength * 0.001f;
            int ms = Mathf.RoundToInt(delta * 1000f);
            float totalDampening = Mathf.Pow(dampeningFactor, ms);
            Vector2 vTotal = velocity * ((totalDampening - 1f) / Mathf.Log(dampeningFactor));

            velocity = velocity * totalDampening;

            return vTotal * 0.06f;
        }

        private Vector2 ClampToScreen(Vector2 position)
        {
            if (this.canvas != null)
            {
                if (this.canvas.renderMode == RenderMode.ScreenSpaceOverlay || this.canvas.renderMode == RenderMode.ScreenSpaceCamera)
                {
                    float clampedX = Mathf.Clamp(position.x, 0f, Screen.width);
                    float clampedY = Mathf.Clamp(position.y, 0f, Screen.height);

                    return new Vector2(clampedX, clampedY);
                }
            }

            // Default
            return position;
        }

        private Vector2 ClampToCanvas(Vector2 position)
        {
            if (this.canvasRectTransform != null)
            {
                Vector3[] corners = new Vector3[4];
                this.canvasRectTransform.GetLocalCorners(corners);

                float clampedX = Mathf.Clamp(position.x, corners[0].x, corners[2].x);
                float clampedY = Mathf.Clamp(position.y, corners[3].y, corners[1].y);

                return new Vector2(clampedX, clampedY);
            }

            // Default
            return position;
        }
    }
}