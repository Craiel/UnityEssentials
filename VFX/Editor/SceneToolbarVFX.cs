namespace Craiel.UnityVFX.Editor
{
    using UnityEditor;
    using UnityEngine;
    using UnityEssentials.Editor;
    using Window;

    public class SceneToolbarVFX : SceneToolbarWidget
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void DrawGUI()
        {
            base.DrawGUI();
            
            if (EditorGUILayout.DropdownButton(new GUIContent("VFX"), FocusType.Passive, "ToolbarDropDown"))
            {
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("VFX Editor"), false, VFXEditorWindow.OpenWindow);
                menu.AddSeparator("");
                menu.ShowAsContext();
                Event.current.Use();
            }
        }
    }
}