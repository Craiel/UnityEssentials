namespace Craiel.TransformRectPlus
{
    using UnityEditor;
    using UnityEngine;
    using UnityEssentials.Editor.TransformPlus;
    using Craiel.UnityEssentials.Editor;
    using Craiel.UnityEssentials.Runtime.Event.Editor;
    using TransformPlus;

    [CustomEditor(typeof(RectTransform))]
    public class TransformRectPlusEditor : EssentialsEditor<TransformRectPlusEditorContainer>
    {
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

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void PostInitializeContainer(TransformRectPlusEditorContainer container)
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
            var activeGameObject = Selection.activeObject as GameObject;
            if (activeGameObject != null)
            {
                TransformRectPlus.SetCurrent(activeGameObject.transform as RectTransform);
            }
            else
            {
                TransformRectPlus.SetCurrent(null);
            }

            EditorEvents.Send(new TransformRectPlusSelectionChanged());
        }
    }
}