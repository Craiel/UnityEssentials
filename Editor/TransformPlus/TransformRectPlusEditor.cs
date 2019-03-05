namespace Craiel.TransformRectPlus
{
    using System;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;
    using UnityEssentials.Editor.TransformPlus;

    [CustomEditor(typeof(RectTransform))]
    public class TransformRectPlusEditor : Editor
    {
        private Editor baseEditor;
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void OnInspectorGUI()
        {
            TransformRectPlus.SetCurrent((RectTransform) this.target);
            if (TransformRectPlus.Current.hasChanged)
            {
                TransformRectPlus.UpdateFromScene();
            }
            
            this.Draw();
        }
        
        public void OnDisable()
        {
            Selection.selectionChanged -= this.OnSelectionChanged;
        }

        public void OnEnable()
        {
            Selection.selectionChanged += this.OnSelectionChanged;
            
            Assembly ass = Assembly.GetAssembly(typeof(UnityEditor.Editor));
            Type editorType = ass.GetType("UnityEditor.RectTransformEditor");
            this.baseEditor = CreateEditor(target, editorType);
        }
        
        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void OnSelectionChanged()
        {
            var target = Selection.activeObject as RectTransform;
            TransformRectPlus.SetCurrent(target);
        }
        
        private void Draw()
        {
            GUISkin resetSkin = GUI.skin;
            GUI.skin = TransformPlusStyles.Skin;

            this.DrawMainControls();

            baseEditor.OnInspectorGUI();
            
            GUILayout.Space(5);

            this.DrawCopy();
            this.DrawPaste();
            this.DrawReset();
        }
        
        private void DrawMainControls()
        {
            EditorGUILayout.BeginHorizontal();
            
            GUI.color = TransformPlusStyles.ColorClear;
            if (GUILayout.Button("Reset", TransformPlusStyles.ButtonBold, GUILayout.Width(60)))
            {
                Undo.RecordObject(TransformRectPlus.Current, "TransformRectPlus Reset");
                TransformRectPlus.Reset();
            }
            
            GUI.color = Color.white;
            EditorGUILayout.EndHorizontal();
        }
        
        private void DrawCopy()
        {
            EditorGUILayout.BeginHorizontal();
            
            GUILayout.Label("Copy", EditorStyles.centeredGreyMiniLabel, GUILayout.Width(60));
            GUI.color = TransformPlusStyles.ColorCopy;
            if (GUILayout.Button("All", TransformPlusStyles.ButtonBold))
            {
                TransformRectPlus.CopyPivot();
                TransformRectPlus.CopyAnchorMin();
                TransformRectPlus.CopyAnchorMax();
                TransformRectPlus.CopyAnchorPosition();
                TransformRectPlus.CopySize();
            }
            
            if (GUILayout.Button("Anchors", TransformPlusStyles.ButtonLeft))
            {
                TransformRectPlus.CopyPivot();
                TransformRectPlus.CopyAnchorMin();
                TransformRectPlus.CopyAnchorMax();
                TransformRectPlus.CopyAnchorPosition();
            }
            
            if (GUILayout.Button("Size", TransformPlusStyles.ButtonRight))
            {
                TransformRectPlus.CopySize();
            }
            
            GUI.color = Color.white;
            EditorGUILayout.EndHorizontal();
        }

        private void DrawPaste()
        {
            EditorGUILayout.BeginHorizontal();
            
            GUILayout.Label("Paste", EditorStyles.centeredGreyMiniLabel, GUILayout.Width(60));
            GUI.color = TransformPlusStyles.ColorPaste;
            GUI.enabled = !TransformRectPlus.IsReadOnly && TransformRectPlus.HasSizeCopy && TransformRectPlus.HasAnchorMinCopy && TransformRectPlus.HasAnchorMaxCopy && TransformRectPlus.HasAnchorPositionCopy;
            if (GUILayout.Button("All", TransformPlusStyles.ButtonBold))
            {
                Undo.RecordObject(TransformRectPlus.Current, "TransformRectPlus Paste All");
                TransformRectPlus.PastePivot();
                TransformRectPlus.PasteAnchorMin();
                TransformRectPlus.PasteAnchorMax();
                TransformRectPlus.PasteAnchorPosition();
                TransformRectPlus.PasteSize();
            }
            
            GUI.enabled = !TransformRectPlus.IsReadOnly && TransformRectPlus.HasAnchorMinCopy && TransformRectPlus.HasAnchorMaxCopy && TransformRectPlus.HasAnchorPositionCopy;
            if (GUILayout.Button("Anchors", TransformPlusStyles.ButtonLeft))
            {
                Undo.RecordObject(TransformRectPlus.Current, "TransformRectPlus Paste Anchors");
                TransformRectPlus.PastePivot();
                TransformRectPlus.PasteAnchorMin();
                TransformRectPlus.PasteAnchorMax();
                TransformRectPlus.PasteAnchorPosition();
            }
            
            GUI.enabled = !TransformRectPlus.IsReadOnly && TransformRectPlus.HasSizeCopy;
            if (GUILayout.Button("Size", TransformPlusStyles.ButtonRight))
            {
                Undo.RecordObject(TransformRectPlus.Current, "TransformRectPlus Paste Size");
                TransformRectPlus.PasteSize();
            }
            
            GUI.enabled = true;
            GUI.color = Color.white;
            
            EditorGUILayout.EndHorizontal();
        }
        
        private void DrawReset()
        {
            EditorGUILayout.BeginHorizontal();
            
            GUILayout.Label("Reset", EditorStyles.centeredGreyMiniLabel, GUILayout.Width(60));
            GUI.color = TransformPlusStyles.ColorPaste;
            GUI.enabled = !TransformRectPlus.IsReadOnly;
            
            if (GUILayout.Button("All", TransformPlusStyles.ButtonBold))
            {
                Undo.RecordObject(TransformRectPlus.Current, "TransformRectPlus Reset Transform");
                TransformRectPlus.ResetPivot();
                TransformRectPlus.ResetAnchorMin();
                TransformRectPlus.ResetAnchorMax();
                TransformRectPlus.ResetAnchorPosition();
                TransformRectPlus.ResetSize();
            }
            
            if (GUILayout.Button("Anchors", TransformPlusStyles.ButtonLeft))
            {
                Undo.RecordObject(TransformRectPlus.Current, "TransformRectPlus Reset Anchors");
                TransformRectPlus.ResetPivot();
                TransformRectPlus.ResetAnchorMin();
                TransformRectPlus.ResetAnchorMax();
                TransformRectPlus.ResetAnchorPosition();
            }
            
            if (GUILayout.Button("Size", TransformPlusStyles.ButtonRight))
            {
                Undo.RecordObject(TransformRectPlus.Current, "TransformRectPlus Reset Size");
                TransformRectPlus.ResetSize();
            }
            
            GUI.enabled = true;
            GUI.color = Color.white;
            
            EditorGUILayout.EndHorizontal();
        }
    }
}