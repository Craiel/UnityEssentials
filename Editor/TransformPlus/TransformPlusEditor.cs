namespace Craiel.UnityEssentials.Editor.TransformPlus
{
    using UnityEditor;
    using UnityEngine;

    //[CustomEditor(typeof(Transform))]
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

        /*private void Draw()
        {
            GUISkin resetSkin = GUI.skin;
            GUI.skin = TransformProStyles.Skin;

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button(TransformProStyles.IconCog, TransformProStyles.ButtonIcon, GUILayout.Width(19)))
            {
                TransformProEditor.ShowPreferences();
            }

            GUI.enabled = TransformPro.CalculateBounds;
            bool showRendererBoundsOld = TransformProPreferences.ShowRendererBounds;
            bool showColliderBoundsOld = TransformProPreferences.ShowColliderBounds;
            bool showMeshFilterBoundsOld = TransformProPreferences.ShowMeshFilterBounds;
            TransformProPreferences.ShowRendererBounds = GUILayout.Toggle(TransformProPreferences.ShowRendererBounds,
                TransformProStyles.IconRenderer, TransformProStyles.ButtonIconLeft, GUILayout.Width(19));
            TransformProPreferences.ShowColliderBounds = GUILayout.Toggle(TransformProPreferences.ShowColliderBounds,
                TransformProStyles.IconCollider, TransformProStyles.ButtonIconMiddle, GUILayout.Width(19));
            TransformProPreferences.ShowMeshFilterBounds = GUILayout.Toggle(
                TransformProPreferences.ShowMeshFilterBounds, TransformProStyles.IconMeshFilter,
                TransformProStyles.ButtonIconRight, GUILayout.Width(19));
            if ((TransformProPreferences.ShowRendererBounds != showRendererBoundsOld)
                || (TransformProPreferences.ShowColliderBounds != showColliderBoundsOld)
                || (TransformProPreferences.ShowMeshFilterBounds != showMeshFilterBoundsOld))
            {
                SceneView.RepaintAll();
            }

            GUI.enabled = true;

            GUI.color = TransformProStyles.ColorSpace;
            GUI.enabled = TransformPro.Space != TransformProSpace.Local;
            if (GUILayout.Button("Local", TransformProStyles.ButtonLeftBold))
            {
                TransformPro.Space = TransformProSpace.Local;
            }

            GUI.enabled = TransformPro.Space != TransformProSpace.World;
            if (GUILayout.Button("World", TransformProStyles.ButtonRightBold))
            {
                TransformPro.Space = TransformProSpace.World;
            }

            GUI.enabled = true;

            GUI.color = TransformProStyles.ColorClear;
            if (GUILayout.Button("Reset", TransformProStyles.ButtonBold, GUILayout.Width(60)))
            {
                Undo.RecordObject(TransformPro.Transform, "TransformPro Reset");
                TransformPro.Reset();
            }

            GUI.color = Color.white;
            EditorGUILayout.EndHorizontal();

            // Display the relative transform picker.
            //if (TransformPro.Space == TransformProSpace.Relative)
            //{
            //    TransformPro.RelativeTransform = EditorGUILayout.ObjectField("Relative Target", TransformPro.RelativeTransform, typeof(Transform), true) as Transform;
            //}

            // ------------------------------------------------------------------------------------------------------------------------------------------
            GUILayout.Space(5);

            Rect positionRect = EditorGUILayout.GetControlRect();
            EditorGUI.BeginChangeCheck();
            GUI.enabled = TransformPro.CanChangePosition;
            positionRect.width -= 20;
            Vector3 position = EditorGUI.Vector3Field(positionRect, "Position", TransformPro.Position);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(TransformPro.Transform, "TransformPro Move");
                TransformPro.Position = position;
            }

            Rect positionClearRect = positionRect;
            positionClearRect.xMin = positionClearRect.xMax;
            positionClearRect.width = 20;
            GUI.color = TransformProStyles.ColorClear;
            if (GUI.Button(positionClearRect, "0", TransformProStyles.Button))
            {
                Undo.RecordObject(TransformPro.Transform, "TransformPro Reset Position");
                TransformPro.ResetPosition();
            }

            GUI.color = Color.white;
            GUI.enabled = true;
            TransformProEditor.advancedPosition =
                EditorGUI.Foldout(positionRect, TransformProEditor.advancedPosition, "");
            if (TransformProEditor.advancedPosition)
            {
                //TransformPro.SnapPositionGrid = EditorGUILayout.Vector3Field("Grid", TransformPro.SnapPositionGrid);

                TransformProEditorAdvanced.DrawPositionGUI();

                // ------------------------------------------------------------------------------------------------------------------------------------------
                GUILayout.Space(5); // space only added to advanced control
            }

            Rect rotationRect = EditorGUILayout.GetControlRect();
            EditorGUI.BeginChangeCheck();
            GUI.enabled = TransformPro.CanChangeRotation;
            rotationRect.width -= 20;
            Vector3 eulerAngles = EditorGUI.Vector3Field(rotationRect, "Rotation", TransformPro.RotationEuler);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(TransformPro.Transform, "TransformPro Rotate");
                TransformPro.RotationEuler = eulerAngles;
            }

            Rect rotationClearRect = rotationRect;
            rotationClearRect.xMin = rotationClearRect.xMax;
            rotationClearRect.width = 20;
            GUI.color = TransformProStyles.ColorClear;
            if (GUI.Button(rotationClearRect, "0", TransformProStyles.Button))
            {
                Undo.RecordObject(TransformPro.Transform, "TransformPro Reset Rotation");
                TransformPro.ResetRotation();
            }

            GUI.color = Color.white;
            GUI.enabled = true;
            TransformProEditor.advancedRotation =
                EditorGUI.Foldout(rotationRect, TransformProEditor.advancedRotation, "");
            if (TransformProEditor.advancedRotation)
            {
                //TransformPro.SnapRotationGrid = EditorGUILayout.Vector3Field("Grid", TransformPro.SnapRotationGrid);

                TransformProEditorAdvanced.DrawRotationGUI();

                // ------------------------------------------------------------------------------------------------------------------------------------------
                GUILayout.Space(5); // space only added to advanced control
            }

            Rect scaleRect = EditorGUILayout.GetControlRect();
            GUI.enabled = TransformPro.CanChangeScale;
            EditorGUI.BeginChangeCheck();
            scaleRect.width -= 20;
            Vector3 localScale = EditorGUI.Vector3Field(scaleRect, "Scale", TransformPro.Scale);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(TransformPro.Transform, "TransformPro Scale");
                TransformPro.Scale = localScale;
            }

            Rect scaleClearRect = scaleRect;
            scaleClearRect.xMin = scaleClearRect.xMax;
            scaleClearRect.width = 20;
            GUI.color = TransformProStyles.ColorClear;
            if (GUI.Button(scaleClearRect, "1", TransformProStyles.Button))
            {
                Undo.RecordObject(TransformPro.Transform, "TransformPro Reset Scale");
                TransformPro.ResetScale();
            }

            GUI.color = Color.white;
            GUI.enabled = true;

            // ------------------------------------------------------------------------------------------------------------------------------------------
            GUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            GUI.color = TransformProStyles.ColorSnap;
            GUI.enabled = TransformPro.CanChangePosition && TransformPro.CanChangeRotation;
            if (GUILayout.Button("Snap", TransformProStyles.ButtonLeftBold))
            {
                Undo.RecordObject(TransformPro.Transform, "TransformPro Snap");
                TransformPro.SnapPositionGrid = TransformProPreferences.SnapPositionGrid;
                TransformPro.SnapRotationGrid = TransformProPreferences.SnapRotationGrid;
                TransformPro.Snap();
            }

            if (GUILayout.Button("Position", TransformProStyles.ButtonMiddle))
            {
                Undo.RecordObject(TransformPro.Transform, "TransformPro Snap Position");
                TransformPro.SnapPositionGrid = TransformProPreferences.SnapPositionGrid;
                TransformPro.Position = TransformPro.SnapPosition(TransformPro.Position);
            }

            if (GUILayout.Button("Rotation", TransformProStyles.ButtonRight))
            {
                Undo.RecordObject(TransformPro.Transform, "TransformPro Snap Rotation");
                TransformPro.SnapRotationGrid = TransformProPreferences.SnapRotationGrid;
                TransformPro.RotationEuler = TransformPro.SnapRotationEuler(TransformPro.RotationEuler);
            }

            GUI.color = Color.white;
            GUI.enabled = TransformPro.CanChangePosition && TransformPro.CalculateBounds;
            GUIContent dropLabel =
                new GUIContent("Drop", "Place the object on the ground without changing the rotation.");
            if (GUILayout.Button(dropLabel, TransformProStyles.ButtonLeftBold))
            {
                Undo.RecordObject(TransformPro.Transform, "TransformPro Drop");
                if (!TransformPro.Ground(false))
                {
                    Debug.LogWarning(
                        "[<color=red>TransformPro</color>] Grounding attempt failed.\nCould not find a collider underneath the transform.");
                }
            }

            GUIContent groundLabel = new GUIContent("Ground", "Place the object on the ground and align the rotation.");
            if (GUILayout.Button(groundLabel, TransformProStyles.ButtonRightBold))
            {
                Undo.RecordObject(TransformPro.Transform, "TransformPro Ground");
                if (!TransformPro.Ground(true))
                {
                    Debug.LogWarning(
                        "[<color=red>TransformPro</color>] Grounding attempt failed.\nCould not find a collider underneath the transform.");
                }
            }

            GUI.color = Color.white;
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();

            // ------------------------------------------------------------------------------------------------------------------------------------------
            GUILayout.Space(5);

            // Copy buttons
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Copy", EditorStyles.centeredGreyMiniLabel, GUILayout.Width(60));
            GUI.color = TransformProStyles.ColorCopy;
            if (GUILayout.Button("Transform", TransformProStyles.ButtonBold))
            {
                TransformPro.CopyPosition();
                TransformPro.CopyRotation();
                TransformPro.CopyScale();
            }

            if (GUILayout.Button("Position", TransformProStyles.ButtonLeft))
            {
                TransformPro.CopyPosition();
            }

            if (GUILayout.Button("Rotation", TransformProStyles.ButtonMiddle))
            {
                TransformPro.CopyRotation();
            }

            if (GUILayout.Button("Scale", TransformProStyles.ButtonRight))
            {
                TransformPro.CopyScale();
            }

            GUI.color = Color.white;
            EditorGUILayout.EndHorizontal();

            // Paste buttons
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Paste", EditorStyles.centeredGreyMiniLabel, GUILayout.Width(60));
            GUI.color = TransformProStyles.ColorPaste;
            GUI.enabled = TransformPro.CanChangePosition || TransformPro.CanChangeRotation ||
                          TransformPro.CanChangeScale;
            if (GUILayout.Button("Transform", TransformProStyles.ButtonBold))
            {
                Undo.RecordObject(TransformPro.Transform, "TransformPro Paste Transform");
                TransformPro.PastePosition();
                TransformPro.PasteRotation();
                TransformPro.PasteScale();
            }

            GUI.enabled = TransformPro.CanChangePosition;
            if (GUILayout.Button("Position", TransformProStyles.ButtonLeft))
            {
                Undo.RecordObject(TransformPro.Transform, "TransformPro Paste Position");
                TransformPro.PastePosition();
            }

            GUI.enabled = TransformPro.CanChangeRotation;
            if (GUILayout.Button("Rotation", TransformProStyles.ButtonMiddle))
            {
                Undo.RecordObject(TransformPro.Transform, "TransformPro Paste Rotate");
                TransformPro.PasteRotation();
            }

            GUI.enabled = TransformPro.CanChangeScale;
            if (GUILayout.Button("Scale", TransformProStyles.ButtonRight))
            {
                Undo.RecordObject(TransformPro.Transform, "TransformPro Paste Scale");
                TransformPro.PasteScale();
            }

            GUI.enabled = true;
            GUI.color = Color.white;
            EditorGUILayout.EndHorizontal();

            GUI.skin = resetSkin;
        }*/
    }
}