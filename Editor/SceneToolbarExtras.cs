namespace Craiel.UnityEssentials.Editor
{
    using UnityEditor;
    using UnityEngine;

    public class SceneToolbarExtras : SceneToolbarWidget
    {
        private const int IconSize = 24;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SceneToolbarExtras()
        {
            this.IsButtonWidget = true;
        }
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void DrawGUI()
        {
            base.DrawGUI();
            
            GUIStyle someGUIStyle = GUI.skin.GetStyle("minibutton");
            someGUIStyle.padding = new RectOffset(1,1,0,0);
            someGUIStyle.overflow = new RectOffset(0,0,2,4);
            someGUIStyle.fixedHeight = IconSize - 4;
            someGUIStyle.imagePosition = ImagePosition.ImageAbove;
            
            GUIContent someGuiContent = new GUIContent();
            someGuiContent.tooltip = someGuiContent.text = "Render & Project Settings";
            
            someGuiContent.text = "";

            someGuiContent.tooltip = "Audio";
            someGuiContent.image = AssetPreview.GetMiniTypeThumbnail(typeof(AudioClip));
            if (GUILayout.Button(someGuiContent, someGUIStyle, GUILayout.Width(IconSize)))
            {
                SettingsService.OpenProjectSettings("Project/Audio");
            }
            
            someGuiContent.tooltip = "Graphics";
            someGuiContent.image = EditorGUIUtility.Load("icons/d_unityeditor.sceneview.png") as Texture2D;
            if (GUILayout.Button(someGuiContent, someGUIStyle, GUILayout.Width(IconSize)))
            {
                SettingsService.OpenProjectSettings("Project/Graphics");
            }
            
            someGuiContent.tooltip = "Input";
            someGuiContent.image = EditorGUIUtility.Load("icons/d_movetool.png") as Texture2D;
            if (GUILayout.Button(someGuiContent, someGUIStyle, GUILayout.Width(IconSize)))
            {
                SettingsService.OpenProjectSettings("Project/Input");
            }
            
            someGuiContent.tooltip = "Player";
            someGuiContent.image = EditorGUIUtility.Load("icons/d_unityeditor.gameview.png") as Texture2D;
            if (GUILayout.Button(someGuiContent, someGUIStyle, GUILayout.Width(IconSize)))
            {
                SettingsService.OpenProjectSettings("Project/Player");
            }
            
            someGuiContent.tooltip = "Quality";
            someGuiContent.image = EditorGUIUtility.Load("icons/d_viewtoolorbit.png") as Texture2D;
            if (GUILayout.Button(someGuiContent, someGUIStyle, GUILayout.Width(IconSize)))
            {
                SettingsService.OpenProjectSettings("Project/Quality");
            }
        }
    }
}