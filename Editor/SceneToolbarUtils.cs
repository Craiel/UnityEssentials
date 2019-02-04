namespace Craiel.UnityEssentials.Editor
{
    using Runtime.Event.Editor;
    using UnityEditor;
    using UnityEngine;

    public class SceneToolbarUtils : SceneToolbarWidget
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void OnGUi()
        {
            base.OnGUi();
            if (GUILayout.Button("Utils", "ToolbarDropDown"))
            {
                var menu = new GenericMenu();
              
                menu.AddItem(new GUIContent("Clean Empty Directories"), false, () =>
                {
                    EditorWindow.GetWindow(typeof(DirectoryUtilsWindow), true, "Directory Utils");
                });

                menu.AddItem(new GUIContent("Search for ComponentFactories"), false, () =>
                {
                    EditorWindow.GetWindow(typeof(SearchForComponentsWindow), true, "Search ComponentFactories");
                });
                
                menu.AddItem(new GUIContent("Game Events Window"), false, GameEventsWindow.OpenWindow);
                
                menu.ShowAsContext();
                Event.current.Use();
            }
        }
    }
}