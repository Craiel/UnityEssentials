namespace Craiel.UnityEssentials.Editor.TransformPlus
{
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(Transform))]
    public class TransformPlusEditor : Editor
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void OnInspectorGUI()
        {
            TransformPlus.SetCurrent((Transform) this.target);
            if (TransformPlus.Current.hasChanged)
            {
                TransformPlus.UpdateFromScene();
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
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void OnSelectionChanged()
        {
            TransformPlus.SetCurrent(Selection.activeTransform);
        }

        private void Draw()
        {
            GUISkin resetSkin = GUI.skin;
            GUI.skin = TransformPlusStyles.Skin;

            this.DrawMainControls();
            
            GUILayout.Space(5);

            this.DrawPosition();
            this.DrawRotation();
            this.DrawScale();

            this.DrawSnap();

            this.DrawCopy();
            this.DrawPaste();
        }

        private void DrawMainControls()
        {
            EditorGUILayout.BeginHorizontal();
            
            GUI.color = TransformPlusStyles.ColorSpace;
            GUI.enabled = TransformPlus.Space != TransformPlusSpace.Local;
            
            if (GUILayout.Button("Local", TransformPlusStyles.ButtonLeftBold))
            {
                TransformPlus.Space = TransformPlusSpace.Local;
            }
            
            GUI.enabled = TransformPlus.Space != TransformPlusSpace.World;
            if (GUILayout.Button("World", TransformPlusStyles.ButtonRightBold))
            {
                TransformPlus.Space = TransformPlusSpace.World;
            }
            
            GUI.enabled = true;

            GUI.color = TransformPlusStyles.ColorClear;
            if (GUILayout.Button("Reset", TransformPlusStyles.ButtonBold, GUILayout.Width(60)))
            {
                Undo.RecordObject(TransformPlus.Current, "TransformPlus Reset");
                TransformPlus.Reset();
            }
            
            GUI.color = Color.white;
            EditorGUILayout.EndHorizontal();
        }

        private void DrawPosition()
        {
            GUI.enabled = !TransformPlus.IsReadOnly;
            
            Rect positionRect = EditorGUILayout.GetControlRect();
            positionRect.width -= 20;
            
            EditorGUI.BeginChangeCheck();
            Vector3 position = EditorGUI.Vector3Field(positionRect, "Position", TransformPlus.Position);
            
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(TransformPlus.Current, "TransformPlus Move");
                TransformPlus.SetPosition(position);
            }
            
            Rect positionClearRect = positionRect;
            positionClearRect.xMin = positionClearRect.xMax;
            positionClearRect.width = 20;
            
            GUI.color = TransformPlusStyles.ColorClear;
            if (GUI.Button(positionClearRect, "0", TransformPlusStyles.Button))
            {
                Undo.RecordObject(TransformPlus.Current, "TransformPlus Reset Position");
                TransformPlus.SetPosition(Vector3.zero);
            }
            
            GUI.color = Color.white;
            GUI.enabled = true;
        }

        private void DrawRotation()
        {
            GUI.enabled = !TransformPlus.IsReadOnly;
            
            Rect rotationRect = EditorGUILayout.GetControlRect();
            EditorGUI.BeginChangeCheck();
            
            rotationRect.width -= 20;
            Vector3 eulerAngles = EditorGUI.Vector3Field(rotationRect, "Rotation", TransformPlus.Rotation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(TransformPlus.Current, "TransformPlus Rotate");
                TransformPlus.SetRotation(eulerAngles);
            }
            
            Rect rotationClearRect = rotationRect;
            rotationClearRect.xMin = rotationClearRect.xMax;
            rotationClearRect.width = 20;
            GUI.color = TransformPlusStyles.ColorClear;
            if (GUI.Button(rotationClearRect, "0", TransformPlusStyles.Button))
            {
                Undo.RecordObject(TransformPlus.Current, "TransformPlus Reset Rotation");
                TransformPlus.SetRotation(Vector3.zero);
            }
            
            GUI.color = Color.white;
            GUI.enabled = true;
        }

        private void DrawScale()
        {
            GUI.enabled = !TransformPlus.IsReadOnly && TransformPlus.Space == TransformPlusSpace.Local;
            
            Rect scaleRect = EditorGUILayout.GetControlRect();
            EditorGUI.BeginChangeCheck();
            scaleRect.width -= 20;
            
            Vector3 localScale = EditorGUI.Vector3Field(scaleRect, "Scale", TransformPlus.Scale);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(TransformPlus.Current, "TransformPlus Scale");
                TransformPlus.SetScale(localScale);
            }
            
            Rect scaleClearRect = scaleRect;
            scaleClearRect.xMin = scaleClearRect.xMax;
            scaleClearRect.width = 20;
            GUI.color = TransformPlusStyles.ColorClear;
            if (GUI.Button(scaleClearRect, "1", TransformPlusStyles.Button))
            {
                Undo.RecordObject(TransformPlus.Current, "TransformPlus Reset Scale");
                TransformPlus.SetScale(Vector3.one);
            }
            
            GUI.color = Color.white;
            GUI.enabled = true;
        }

        private void DrawSnap()
        {
            GUILayout.Space(5);

            GUI.enabled = !TransformPlus.IsReadOnly;
            
            EditorGUILayout.BeginHorizontal();
            GUI.color = TransformPlusStyles.ColorSnap;
            
            if (GUILayout.Button("Snap", TransformPlusStyles.ButtonLeftBold))
            {
                Undo.RecordObject(TransformPlus.Current, "TransformPlus Snap");
                TransformPlus.SetPositionSnapped(TransformPlus.Position);
                TransformPlus.SetRotationSnapped(TransformPlus.Rotation);
            }
            
            if (GUILayout.Button("Position", TransformPlusStyles.ButtonMiddle))
            {
                Undo.RecordObject(TransformPlus.Current, "TransformPlus Snap Position");
                TransformPlus.SetPositionSnapped(TransformPlus.Position);
            }
            
            if (GUILayout.Button("Rotation", TransformPlusStyles.ButtonRight))
            {
                Undo.RecordObject(TransformPlus.Current, "TransformPlus Snap Rotation");
                TransformPlus.SetRotationSnapped(TransformPlus.Rotation);
            }
            
            GUI.color = Color.white;
            GUI.enabled = true;
            
            EditorGUILayout.EndHorizontal();
        }

        private void DrawCopy()
        {
            EditorGUILayout.BeginHorizontal();
            
            GUILayout.Label("Copy", EditorStyles.centeredGreyMiniLabel, GUILayout.Width(60));
            GUI.color = TransformPlusStyles.ColorCopy;
            if (GUILayout.Button("Transform", TransformPlusStyles.ButtonBold))
            {
                TransformPlus.CopyPosition();
                TransformPlus.CopyRotation();
                TransformPlus.CopyScale();
            }
            
            if (GUILayout.Button("Position", TransformPlusStyles.ButtonLeft))
            {
                TransformPlus.CopyPosition();
            }
            
            if (GUILayout.Button("Rotation", TransformPlusStyles.ButtonMiddle))
            {
                TransformPlus.CopyRotation();
            }
            
            if (GUILayout.Button("Scale", TransformPlusStyles.ButtonRight))
            {
                TransformPlus.CopyScale();
            }
            
            GUI.color = Color.white;
            EditorGUILayout.EndHorizontal();
        }

        private void DrawPaste()
        {
            EditorGUILayout.BeginHorizontal();
            
            GUILayout.Label("Paste", EditorStyles.centeredGreyMiniLabel, GUILayout.Width(60));
            GUI.color = TransformPlusStyles.ColorPaste;
            GUI.enabled = !TransformPlus.IsReadOnly && TransformPlus.HasScaleCopy && TransformPlus.HasPositionCopy && TransformPlus.HasRotationCopy;
            if (GUILayout.Button("Transform", TransformPlusStyles.ButtonBold))
            {
                Undo.RecordObject(TransformPlus.Current, "TransformPlus Paste Transform");
                TransformPlus.PastePosition();
                TransformPlus.PasteRotation();
                TransformPlus.PasteScale();
            }
            
            GUI.enabled = !TransformPlus.IsReadOnly && TransformPlus.HasPositionCopy;
            if (GUILayout.Button("Position", TransformPlusStyles.ButtonLeft))
            {
                Undo.RecordObject(TransformPlus.Current, "TransformPlus Paste Position");
                TransformPlus.PastePosition();
            }
            
            GUI.enabled = !TransformPlus.IsReadOnly && TransformPlus.HasRotationCopy;
            if (GUILayout.Button("Rotation", TransformPlusStyles.ButtonMiddle))
            {
                Undo.RecordObject(TransformPlus.Current, "TransformPlus Paste Rotate");
                TransformPlus.PasteRotation();
            }
            
            GUI.enabled = !TransformPlus.IsReadOnly && TransformPlus.HasScaleCopy;
            if (GUILayout.Button("Scale", TransformPlusStyles.ButtonRight))
            {
                Undo.RecordObject(TransformPlus.Current, "TransformPlus Paste Scale");
                TransformPlus.PasteScale();
            }
            
            GUI.enabled = true;
            GUI.color = Color.white;
            
            EditorGUILayout.EndHorizontal();
        }
    }
}