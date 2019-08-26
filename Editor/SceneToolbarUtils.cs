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
        public override void DrawGUI()
        {
            base.DrawGUI();

            if (GUILayout.Button("Utils", "ToolbarDropDown"))
            {
                var menu = new GenericMenu();

                menu.AddItem(new GUIContent("Clean Empty Directories"), false, DirectoryUtilsWindow.OpenWindow);

                menu.AddItem(new GUIContent("Search for ComponentFactories"), false, SearchForComponentsWindow.OpenWindow);

                menu.AddItem(new GUIContent("Game Events Window"), false, GameEventsWindow.OpenWindow);

                menu.ShowAsContext();
                Event.current.Use();
            }
        }
    }
}