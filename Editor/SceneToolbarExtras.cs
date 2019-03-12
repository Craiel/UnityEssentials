namespace Craiel.UnityEssentials.Editor
{
    using UnityEditor;
    using UnityEngine;

    public class SceneToolbarExtras : SceneToolbarWidget
    {
        private const int IconSize = 24;

        private GUIStyle guiStyle;
        
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

            if (this.guiStyle == null)
            {
                this.guiStyle = GUI.skin.GetStyle("minibutton");
                this.guiStyle.padding = new RectOffset(1, 1, 0, 0);
                this.guiStyle.overflow = new RectOffset(0, 0, 2, 4);
                this.guiStyle.fixedHeight = IconSize - 4;
                this.guiStyle.imagePosition = ImagePosition.ImageAbove;
            }

            GUIContent guiContent = new GUIContent();
            guiContent.tooltip = guiContent.text = "Render & Project Settings";
            
            guiContent.text = "";

            guiContent.tooltip = "Audio";
            guiContent.image = AssetPreview.GetMiniTypeThumbnail(typeof(AudioClip));
            if (GUILayout.Button(guiContent, this.guiStyle, GUILayout.Width(IconSize)))
            {
                SettingsService.OpenProjectSettings("Project/Audio");
            }
            
            guiContent.tooltip = "Graphics";
            guiContent.image = EditorGUIUtility.Load("icons/d_unityeditor.sceneview.png") as Texture2D;
            if (GUILayout.Button(guiContent, this.guiStyle, GUILayout.Width(IconSize)))
            {
                SettingsService.OpenProjectSettings("Project/Graphics");
            }
            
            guiContent.tooltip = "Input";
            guiContent.image = EditorGUIUtility.Load("icons/d_movetool.png") as Texture2D;
            if (GUILayout.Button(guiContent, this.guiStyle, GUILayout.Width(IconSize)))
            {
                SettingsService.OpenProjectSettings("Project/Input");
            }
            
            guiContent.tooltip = "Player";
            guiContent.image = EditorGUIUtility.Load("icons/d_unityeditor.gameview.png") as Texture2D;
            if (GUILayout.Button(guiContent, this.guiStyle, GUILayout.Width(IconSize)))
            {
                SettingsService.OpenProjectSettings("Project/Player");
            }
            
            guiContent.tooltip = "Quality";
            guiContent.image = EditorGUIUtility.Load("icons/d_viewtoolorbit.png") as Texture2D;
            if (GUILayout.Button(guiContent, this.guiStyle, GUILayout.Width(IconSize)))
            {
                SettingsService.OpenProjectSettings("Project/Quality");
            }
        }
    }
}