namespace Craiel.UnityEssentials.Editor.CameraUtils
{
    using Runtime.CameraUtils;
    using UnityEditor;
    using UnityEngine;
    using UserInterface;

    [CustomEditor(typeof(PixelPerfectCamera))]
    public class PixelPerfectCameraEditor : EssentialCustomEditor
    {
        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void DrawFull()
        {
            base.DrawFull();

            PixelPerfectCamera typedTarget = (PixelPerfectCamera) this.target;
            
            this.DrawProperty<PixelPerfectCamera>(x => x.AssetsPPU);
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Reference Resolution");
            EditorGUIUtility.labelWidth = 30;
            EditorGUILayout.IntField("X", typedTarget.RefResolutionX);
            EditorGUILayout.IntField("Y", typedTarget.RefResolutionY);
            Layout.SetDefaultLabelSize();
            EditorGUILayout.EndHorizontal();
            
            this.DrawProperty<PixelPerfectCamera>(x => x.UpscaleRT);
            if (!typedTarget.UpscaleRT)
            {
                this.DrawProperty<PixelPerfectCamera>(x => x.PixelSnapping);
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Crop Frame");
            EditorGUIUtility.labelWidth = 30;
            EditorGUILayout.Toggle("X", typedTarget.CropFrameX);
            EditorGUILayout.Toggle("Y", typedTarget.CropFrameY);
            Layout.SetDefaultLabelSize();
            EditorGUILayout.EndHorizontal();

            bool newEditMode = GUILayout.Toggle(typedTarget.runInEditMode, "Run in Edit Mode", "Button");
            if (newEditMode != typedTarget.runInEditMode)
            {
                if (newEditMode)
                {
                    typedTarget.EnableEditMode();
                }
                else
                {
                    typedTarget.DisableEditMode();
                }
            }
            
            if (typedTarget.runInEditMode)
            {
                GUI.enabled = false;
                EditorGUILayout.LabelField("Current Pixel Ratio: " + typedTarget.PixelRatio);
                GUI.enabled = true;
            }
        }
    }
}