namespace Craiel.UnityEssentials.Editor
{
    using System;
    using Runtime;
    using UnityEditor;
    using UnityEngine;

    public abstract class EssentialEditorWindow<T, TC> : EditorWindow
        where T : EssentialEditorWindow<T, TC>
        where TC: EssentialEditorTemplateContainer
    {
        private TC container;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static T Instance { get; private set; }

        public virtual void OnEnable()
        {
            this.container = Activator.CreateInstance<TC>();
            this.container.Initialize();
            this.rootVisualElement.Add(this.container);

            Instance = (T)this;
        }

        public virtual void OnDestroy()
        {
            Instance = null;
        }

        public virtual void OnDisable()
        {
            Instance = null;
        }

        public virtual void OnSelectionChange()
        {
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected static void OpenWindow(string windowTitle)
        {
            var window = (T)GetWindow(TypeCache<T>.Value);
            window.titleContent = new GUIContent(windowTitle);
            window.Show();
        }
    }
}