namespace Craiel.UnityEssentials.Runtime.Debug.Gym
{
    using System.Collections.Generic;
    using Input;
    using UnityEngine;

    public class DebugGymSelectorBase<T> : DebugGymBase
        where T : MonoBehaviour
    {
        private const int MaxGroups = 5;

        private float lastTargetUpdateTime;

        private readonly IList<T>[] selectionGroups;

        private readonly IDictionary<T, int> selectionGroupMap;

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        public DebugGymSelectorBase()
        {
            this.AllowGroupSelection = true;

            this.selectionGroups = new IList<T>[MaxGroups];
            for (var i = 0; i < MaxGroups; i++)
            {
                this.selectionGroups[i] = new List<T>();
            }

            this.selectionGroupMap = new Dictionary<T, int>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SerializeField]
        public float SelectDistance = 100.0f;

        [SerializeField]
        public float TargetUpdateInterval = 0.1f;

        [SerializeField]
        public Color HighlightColor = Color.green;

        [SerializeField]
        public Color SelectionColorGrp1 = Color.red;

        [SerializeField]
        public Color SelectionColorGrp2 = Color.blue;

        [SerializeField]
        public Color SelectionColorGrp3 = Color.cyan;

        [SerializeField]
        public Color SelectionColorGrp4 = Color.magenta;

        [SerializeField]
        public Color SelectionColorGrp5 = Color.yellow;

        [SerializeField]
        public bool DrawAllGroups;

        [SerializeField]
        public int RayLayerMask;

        public bool AllowGroupSelection { get; protected set; }

        public int ActiveGroup { get; private set; }

        public T CurrentHighlightTarget { get; private set; }

        public bool HasActiveSelection
        {
            get { return this.HasSelection(this.ActiveGroup); }
        }

        public bool HasSelection(int group)
        {
            return this.selectionGroups[group].Count > 0;
        }

        public void ClearSelection()
        {
            foreach (T entry in this.selectionGroups[this.ActiveGroup])
            {
                this.UnmarkTarget(entry);
                this.selectionGroupMap.Remove(entry);
            }

            this.selectionGroups[this.ActiveGroup].Clear();
        }

        public IList<T> GetActiveSelection()
        {
            return this.GetSelection(this.ActiveGroup);
        }

        public IList<T> GetSelection(int group)
        {
            return new List<T>(this.selectionGroups[group]);
        }

        public void SwitchGroup(int index)
        {
            if (index < 0 || index >= MaxGroups)
            {
                return;
            }

            if (!this.DrawAllGroups)
            {
                this.UnmarkActiveGroup();
            }

            this.ActiveGroup = index;

            if (!this.DrawAllGroups)
            {
                this.MarkActiveGroup();
            }
        }

        public virtual void Update()
        {
            if (Time.time > this.lastTargetUpdateTime + this.TargetUpdateInterval)
            {
                this.lastTargetUpdateTime = Time.time;
                this.UpdateViewTarget();
            }

            if (this.AllowGroupSelection)
            {
                if (InputHandler.Instance.GetControl(InputStateDebug.DebugSelect1).IsUp)
                {
                    this.SwitchGroup(0);
                }
                else if (InputHandler.Instance.GetControl(InputStateDebug.DebugSelect2).IsUp)
                {
                    this.SwitchGroup(1);
                }
                else if (InputHandler.Instance.GetControl(InputStateDebug.DebugSelect3).IsUp)
                {
                    this.SwitchGroup(2);
                }
                else if (InputHandler.Instance.GetControl(InputStateDebug.DebugSelect4).IsUp)
                {
                    this.SwitchGroup(3);
                }
                else if (InputHandler.Instance.GetControl(InputStateDebug.DebugSelect5).IsUp)
                {
                    this.SwitchGroup(4);
                }
            }

            // Target selection
            if (InputHandler.Instance.GetControl(InputStateDebug.DebugConfirm).IsUp)
            {
                if (this.CurrentHighlightTarget != null)
                {
                    if (!this.IsSelected(this.CurrentHighlightTarget))
                    {
                        this.SelectTarget(this.CurrentHighlightTarget);
                    }
                }
                else
                {
                    this.SelectEmpty();
                }
            }
        }

        protected virtual void SelectTarget(T target)
        {
            this.AddToGroup(target);
        }

        protected virtual void SelectEmpty()
        {
            this.ClearSelection();
        }

        protected virtual void MarkTarget(T target, Color color)
        {
        }

        protected virtual void UnmarkTarget(T target)
        {
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private bool IsSelected(T target)
        {
            return this.selectionGroupMap.ContainsKey(target);
        }

        private void AddToGroup(T target)
        {
            this.selectionGroupMap.Add(target, this.ActiveGroup);
            this.selectionGroups[this.ActiveGroup].Add(target);

            this.MarkTarget(target, this.GetActiveGroupColor());
        }

        private Color GetActiveGroupColor()
        {
            return this.GetGroupColor(this.ActiveGroup);
        }

        private Color GetGroupColor(int group)
        {
            switch (group)
            {
                case 0:
                    {
                        return this.SelectionColorGrp1;
                    }

                case 1:
                    {
                        return this.SelectionColorGrp2;
                    }

                case 2:
                    {
                        return this.SelectionColorGrp3;
                    }

                case 3:
                    {
                        return this.SelectionColorGrp4;
                    }

                case 4:
                    {
                        return this.SelectionColorGrp5;
                    }

                default:
                    {
                        return Color.black;
                    }
            }
        }

        private void UnmarkActiveGroup()
        {
            foreach (T entry in this.selectionGroups[this.ActiveGroup])
            {
                this.UnmarkTarget(entry);
            }
        }

        private void MarkActiveGroup()
        {
            foreach (T entry in this.selectionGroups[this.ActiveGroup])
            {
                this.MarkTarget(entry, this.GetActiveGroupColor());
            }
        }

        private T GetRayCastResult(RaycastHit hit)
        {
            if (hit.collider == null || hit.collider.transform == null)
            {
                return null;
            }

            var colliderObject = hit.collider.gameObject;
            var animator = colliderObject.GetComponent<T>();
            if (animator != null)
            {
                return animator;
            }

            animator = colliderObject.GetComponentInChildren<T>();
            if (animator != null)
            {
                return animator;
            }

            return colliderObject.GetComponentInParent<T>();
        }

        private void UpdateViewTarget()
        {
            T newTarget = null;
            Ray ray = new Ray
            {
                origin = this.Controller.transform.position,
                direction = this.Controller.transform.forward
            };

            RaycastHit rayResult;
            if (Physics.Raycast(ray.origin, ray.direction, out rayResult, this.SelectDistance, this.RayLayerMask, QueryTriggerInteraction.Collide))
            {
                newTarget = this.GetRayCastResult(rayResult);
                if (newTarget == null)
                {
                    // No change
                    return;
                }
            }

            if (newTarget == this.CurrentHighlightTarget)
            {
                return;
            }

            if (this.CurrentHighlightTarget != null)
            {
                int currentTargetGroup;
                if (this.selectionGroupMap.TryGetValue(this.CurrentHighlightTarget, out currentTargetGroup))
                {
                    this.MarkTarget(this.CurrentHighlightTarget, this.GetGroupColor(currentTargetGroup));
                }
                else
                {
                    this.UnmarkTarget(this.CurrentHighlightTarget);
                }
            }

            this.CurrentHighlightTarget = newTarget;

            if (this.CurrentHighlightTarget != null)
            {
                this.MarkTarget(this.CurrentHighlightTarget, this.HighlightColor);
            }
        }
    }
}
