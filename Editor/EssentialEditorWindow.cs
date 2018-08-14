namespace Craiel.UnityEssentials.Editor
{
    using UnityEditor;
    using UnityEngine;

    public abstract class EssentialEditorWindow<T> : EditorWindow
        where T : EssentialEditorWindow<T>
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
            var window = (T)GetWindow(typeof(T));
            window.titleContent = new GUIContent(windowTitle);
            window.Show();
        }
    }
}