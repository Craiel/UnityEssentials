namespace Craiel.UnityEssentials.Editor
{
    using Runtime;
    using Runtime.Extensions;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(BoxColliderHelper))]
    public class BoxColliderHelperEditor : EssentialEditorIM
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void OnInspectorGUI()
        {
            var typedTarget = (BoxColliderHelper)this.target;
            var collider = typedTarget.GetComponent<BoxCollider2D>();
            if (collider == null)
            {
                EditorGUILayout.HelpBox("No BoxCollider2D Present.", MessageType.Error);
                return;
            }
            
            this.Draw(typedTarget);
            
            this.serializedObject.ApplyModifiedProperties();
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void Draw(BoxColliderHelper typedTarget)
        {
            typedTarget.SnapToGrid = EditorGUILayout.Toggle("Snap to Grid", typedTarget.SnapToGrid);

            if (typedTarget.SnapToGrid)
            {
                this.UpdateSnapping(typedTarget);
                
                
            }

            GUI.enabled = false;
            this.DrawProperty<BoxColliderHelper>(x => x.ColliderRect);
            GUI.enabled = true;
        }
        
        private void UpdateSnapping(BoxColliderHelper typedTarget)
        {
            Grid targetGrid = typedTarget.gameObject.FindInParents<Grid>();
            if (targetGrid == null)
            {
                EditorGUILayout.HelpBox("Collider has no Grid in parent Hierarchy.", MessageType.Warning);
                return;
            }

            var collider = typedTarget.GetComponent<BoxCollider2D>();
            collider.size = EditorGUILayout.Vector2Field("Size", collider.size);
            
            Vector2 offset;
            offset.x = collider.size.x * (targetGrid.cellSize.x / 2);
            offset.y = collider.size.y * (targetGrid.cellSize.y / 2);
            collider.offset = offset;
        }
    }
}