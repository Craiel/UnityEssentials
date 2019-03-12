namespace Craiel.UnityEssentials.Editor
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    [InitializeOnLoad]
    public static class SceneToolbar
    {
        private static readonly List<SceneToolbarWidget> Widgets;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        static SceneToolbar()
        {
            if (Widgets == null)
            {
                Widgets = new List<SceneToolbarWidget>();
            }

            SceneView.onSceneGUIDelegate -= OnSceneGUI;
            SceneView.onSceneGUIDelegate += OnSceneGUI;

            EditorApplication.playModeStateChanged -= PlaymodeStateChanged;
            EditorApplication.playModeStateChanged += PlaymodeStateChanged;

            EditorApplication.update -= Update;
            EditorApplication.update += Update;

            EssentialsEditorCore.Initialize();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static void RegisterWidget<T>()
            where T : SceneToolbarWidget
        {
            Widgets.Add(Activator.CreateInstance<T>());
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private static void Update()
        {
            foreach (SceneToolbarWidget widget in Widgets)
            {
                widget.Update();
            }
        }

        private static void PlaymodeStateChanged(PlayModeStateChange playModeStateChange)
        {
            foreach (SceneToolbarWidget widget in Widgets)
            {
                widget.PlaymodeStateChanged(playModeStateChange);
            }
        }

        private static void OnSceneGUI(SceneView sceneView)
        {
            if (Application.isPlaying)
            {
                return;
            }

            foreach (SceneToolbarWidget widget in Widgets)
            {
                widget.DrawSceneGUI(sceneView);
            }

            DrawToolBar(sceneView);
        }

        private static void DrawToolBar(SceneView sceneView)
        {
            Handles.BeginGUI();
            var rect = sceneView.position;
            rect.height = 17;
            rect.x = 0;
            rect.y = 0;
            GUILayout.BeginArea(rect);
            EditorGUILayout.BeginHorizontal("toolbar");

            bool hasButtons = false;
            foreach (SceneToolbarWidget widget in Widgets)
            {
                if (widget.IsButtonWidget)
                {
                    hasButtons = true;
                    continue;
                }
                
                widget.DrawGUI();
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            GUILayout.EndArea();

            if (hasButtons)
            {
                rect.y = 20;
                rect.height = 24;
                GUILayout.BeginArea(rect);
                EditorGUILayout.BeginHorizontal();
                
                foreach (SceneToolbarWidget widget in Widgets)
                {
                    if (!widget.IsButtonWidget)
                    {
                        continue;
                    }
                
                    widget.DrawGUI();
                }
                
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
                GUILayout.EndArea();
            }
            Handles.EndGUI();
        }
    }
}