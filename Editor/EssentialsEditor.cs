namespace Craiel.UnityEssentials.Editor
{
    using System;
    using UnityEditor;
    using UnityEngine.UIElements;

    [CanEditMultipleObjects]
    public abstract class EssentialsEditor<T> : UnityEditor.Editor
        where T : EssentialEditorTemplateContainer
    {
        private T activeContainer;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override VisualElement CreateInspectorGUI()
        {
            if (this.activeContainer != null)
            {
                UnityEngine.Debug.LogWarning("Container is already active!");
                return this.activeContainer;
            }

            this.activeContainer = Activator.CreateInstance<T>();
            this.activeContainer.Initialize();

            this.PostInitializeContainer(this.activeContainer);

            return this.activeContainer;
        }

        public virtual void OnDisable()
        {
            this.activeContainer.Dispose();
            this.activeContainer = null;
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected virtual void PostInitializeContainer(T container)
        {
        }
    }
}