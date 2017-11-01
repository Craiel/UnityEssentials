namespace Assets.Scripts.Craiel.Essentials.Editor
{
    using System.Collections.Generic;
    using IO;
    using UnityEditor;
    using UnityEngine;

    public class DirectoryUtilsWindow : EditorWindow
    {
        private const float DirectoryLabelHeight = 21;

        private readonly List<CarbonDirectory> directoryList = new List<CarbonDirectory>();

        private Vector2 scrollPosition;
        private string delayedNotificationMessage;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void OnEnable()
        {
            DirectoryUtils.OnAutoClean += this.OnAutoClean;

            this.delayedNotificationMessage = "Click 'Find Empty Dirs' Button.";
        }

        public void OnDisable()
        {
            DirectoryUtils.OnAutoClean -= this.OnAutoClean;
        }
        
        public void OnGUI()
        {
            if (this.delayedNotificationMessage != null)
            {
                this.ShowNotification(new GUIContent(this.delayedNotificationMessage));
                this.delayedNotificationMessage = null;
            }

            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Find Empty Directories"))
                    {
                        DirectoryUtils.FillEmptyDirList(this.directoryList);

                        if (this.directoryList.Count <= 0)
                        {
                            this.ShowNotification(new GUIContent("No Empty Directory"));
                        }
                        else
                        {
                            this.RemoveNotification();
                        }
                    }
                    
                    if (this.ColorButton("Delete All", this.directoryList.Count > 0, Color.red))
                    {
                        DirectoryUtils.DeleteAllEmptyDirAndMeta(this.directoryList);
                        this.ShowNotification(new GUIContent("Deleted All"));
                    }
                }

                EditorGUILayout.EndHorizontal();

                bool previousSetting = DirectoryUtils.CleanOnSaveEnabled;
                bool newSetting = GUILayout.Toggle(previousSetting, " Clean Empty Dirs Automatically On Save");
                if (newSetting != previousSetting)
                {
                    DirectoryUtils.CleanOnSaveEnabled = newSetting;
                }

                GUILayout.Box(string.Empty, GUILayout.ExpandWidth(true), GUILayout.Height(1));

                if (this.directoryList.Count > 0)
                {
                    this.scrollPosition = EditorGUILayout.BeginScrollView(this.scrollPosition, GUILayout.ExpandWidth(true));
                    {
                        EditorGUILayout.BeginVertical();
                        {
                            GUIContent folderContent = EditorGUIUtility.IconContent("Folder Icon");

                            foreach (CarbonDirectory directory in this.directoryList)
                            {
                                UnityEngine.Object assetObj = AssetDatabase.LoadAssetAtPath("Assets", typeof(UnityEngine.Object));
                                if (null != assetObj)
                                {
                                    folderContent.text = directory.ToRelative<CarbonDirectory>(DirectoryUtils.DataPath).GetPath();
                                    GUILayout.Label(folderContent, GUILayout.Height(DirectoryLabelHeight));
                                }
                            }

                        }

                        EditorGUILayout.EndVertical();
                    }

                    EditorGUILayout.EndScrollView();
                }
            }

            EditorGUILayout.EndVertical();
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private bool ColorButton(string buttonTitle, bool enabled, Color color)
        {
            bool oldEnabled = GUI.enabled;
            Color oldColor = GUI.color;

            GUI.enabled = enabled;
            GUI.color = color;

            bool ret = GUILayout.Button(buttonTitle);

            GUI.enabled = oldEnabled;
            GUI.color = oldColor;

            return ret;
        }

        private void OnAutoClean()
        {
            this.delayedNotificationMessage = "Cleaned on Save";
        }
    }
}
