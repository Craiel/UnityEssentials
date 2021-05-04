namespace Craiel.UnityEssentialsUI.Editor.Theme
{
    using Runtime.Theme;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;

    [CustomEditor(typeof(UITheme))]
    public class ThemeEditor : Editor
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();

            if (GUILayout.Button("Apply Theme"))
            {
                UITheme theme = this.target as UITheme;

                if (theme != null)
                {
                    if (UIThemeSystem.IsInstanceActive)
                    {
                        UIThemeSystem.Instance.SetTheme(theme);
                    }
                    else
                    {
                        UIThemeSystem system = FindObjectOfType<UIThemeSystem>();
                        if (system != null)
                        {
                            system.SetTheme(theme);
                        }
                    }
                    
                    UIThemeSystem.Instance.ApplyTheme();

                    if (!Application.isPlaying)
                    {
                        EditorSceneManager.MarkAllScenesDirty();
                    }
                }
            }
        }
    }
}