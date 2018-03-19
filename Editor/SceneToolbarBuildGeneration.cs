namespace Craiel.UnityEssentials.Editor
{
    using UnityEditor;
    using UnityEngine;

    public class SceneToolbarBuildGeneration : SceneToolbarWidget
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void OnGUi()
        {
            base.OnGUi();
            if (GUILayout.Button("Build", "ToolbarDropDown"))
            {
                var menu = new GenericMenu();
                
                menu.AddItem(new GUIContent("Windows Client - Debug"), false, () =>
                {
                    if (ConfirmBuild())
                    {
                        UnityBuildGenerator.BuildWindowsClientDebug();
                    }
                });

                menu.AddItem(new GUIContent("Windows Client - Release"), false, () =>
                {
                    if (ConfirmBuild())
                    {
                        UnityBuildGenerator.BuildWindowsClientRelease();
                    }
                });

                menu.AddSeparator("");

                menu.AddItem(new GUIContent("Android Client - Debug"), false, () =>
                {
                    if (ConfirmBuild())
                    {
                        UnityBuildGenerator.BuildAndroidClientDebug();
                    }
                });

                menu.AddItem(new GUIContent("Android Client - Release"), false, () =>
                {
                    if (ConfirmBuild())
                    {
                        UnityBuildGenerator.BuildAndroidClientRelease();
                    }
                });

                menu.ShowAsContext();
                Event.current.Use();
            }
        }

        private static bool ConfirmBuild()
        {
            return EditorUtility.DisplayDialog("Confirm Build", "Building will change open scenes, proceed?", "Ok", "Cancel");
        }
    }
}
