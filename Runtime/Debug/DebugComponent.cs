using GizmoUtils = Craiel.UnityEssentials.Runtime.Utils.GizmoUtils;

namespace Craiel.UnityEssentials.Runtime.Debug
{
    using System.Text;
    using UnityEngine;

    public abstract class DebugComponent : MonoBehaviour
    {
        private readonly StringBuilder tempStringBuilder;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        protected DebugComponent()
        {
            this.tempStringBuilder = new StringBuilder();
            this.Enable = true;
            this.EnableLabels = false;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SerializeField]
        public bool Enable;

        [SerializeField]
        public bool EnableLabels;

        [SerializeField]
        public bool ColliderGizmos;

        [SerializeField]
        public Color ColliderBoxColor = Color.green;

        [SerializeField]
        public Color ColliderSphereColor = Color.green;

        [SerializeField]
        public Color ColliderUnsupportedTypeColor = Color.magenta;

        public virtual void Start()
        {
            this.Main = this.GetComponentInParent<DebugMainBase>();
        }

        public virtual void Update()
        {
            if (!this.Enable)
            {
                return;
            }

            this.DoUpdate();
        }

        public void OnDrawGizmos()
        {
            if (!this.Enable)
            {
                return;
            }

            this.DrawGizmos();
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected DebugMainBase Main { get; private set; }

        protected abstract void DoUpdate();

        protected abstract void DrawGizmos();

        protected virtual void DrawGui()
        {
        }

        protected void DrawColliders(GameObject source)
        {
            if (!this.ColliderGizmos)
            {
                return;
            }

            Collider[] colliders = source.GetComponentsInChildren<Collider>();
            this.tempStringBuilder.Length = 0;
            for (var i = 0; i < colliders.Length; i++)
            {
                Collider current = colliders[i];
                if (!current.enabled || !current.gameObject.activeInHierarchy)
                {
                    continue;
                }

                var box = current as BoxCollider;
                if (box != null)
                {
                    GizmoUtils.DrawCube(box.bounds, this.ColliderBoxColor);
                    continue;
                }

                var sphere = current as SphereCollider;
                if (sphere != null)
                {
                    GizmoUtils.DrawSphere(sphere.center, sphere.radius, this.ColliderSphereColor);
                    continue;
                }

                this.tempStringBuilder.AppendLine(string.Format("C: {0}", current.GetType().Name));
            }

            if (this.tempStringBuilder.Length > 0)
            {
                GizmoUtils.DrawLabel(source.gameObject.transform.position, this.tempStringBuilder.ToString(), this.ColliderUnsupportedTypeColor);
            }
        }
    }
}
