namespace Craiel.UnityEssentials.Editor.TransformPlus
{
    using Craiel.UnityEssentials.Runtime.Event.Editor;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(Transform))]
    public class TransformPlusEditor : EssentialsEditor<TransformPlusEditorContainer>
    {
        private static readonly Color PivotColor = new Color(255, 165, 0);

        private static System.Action imExtension;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void OnDisable()
        {
            Selection.selectionChanged -= this.OnSelectionChanged;

            base.OnDisable();
        }

        public void OnEnable()
        {
            Selection.selectionChanged += this.OnSelectionChanged;
        }

        public static void SetExtension(System.Action onGuiCallback)
        {
            imExtension = onGuiCallback;
        }

        [DrawGizmo(GizmoType.Selected | GizmoType.Active)]
        public static void DrawPivot(Transform target, GizmoType type)
        {
            if (!TransformPlus.DrawPivot || TransformPlus.Current == null)
            {
                return;
            }

            Gizmos.matrix = TransformPlus.Current.localToWorldMatrix;
            Gizmos.color = PivotColor;
            Gizmos.DrawWireSphere(Vector3.zero, 0.05f);
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void PostInitializeContainer(TransformPlusEditorContainer container)
        {
            base.PostInitializeContainer(container);

            if (imExtension != null)
            {
                container.AddCustomIMGUIContent(imExtension);
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void OnSelectionChanged()
        {
            TransformPlus.SetCurrent(Selection.activeTransform);
            EditorEvents.Send(new TransformPlusSelectionChanged());
        }
    }
}