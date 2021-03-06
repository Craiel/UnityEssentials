﻿namespace Craiel.UnityEssentials.Editor
{
    using Runtime;
    using UnityEditor;
    using UnityEngine;

    public abstract class EssentialEditorWindowIM<T> : EditorWindow
        where T : EssentialEditorWindowIM<T>
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static T Instance { get; private set; }

        [SerializeField]
        public GUIStyle ToolBarStyle;

        public virtual void OnEnable()
        {
            Instance = (T)this;

            if (this.ToolBarStyle == null)
            {
                this.ToolBarStyle = new GUIStyle(EditorStyles.toolbarButton)
                {
                    imagePosition = ImagePosition.ImageOnly,
                    fixedHeight = 48,
                    fixedWidth = 48,
                    wordWrap = false
                };
            }
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