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
            foreach (var w in Widgets)
            {
                w.Update();
            }
        }

        private static void PlaymodeStateChanged(PlayModeStateChange playModeStateChange)
        {
            foreach (var widget in Widgets)
            {
                widget.PlaymodeStateChanged(playModeStateChange);
            }
        }

        private static void OnSceneGUI(SceneView sceneView)
        {
            foreach (var w in Widgets)
            {
                w.OnSceneGUI(sceneView);
            }

            Handles.BeginGUI();
            var rect = sceneView.position;
            rect.height = 17;
            rect.x = 0;
//            rect.y = sceneView.position.height - 34;
            rect.y = 0;
            GUILayout.BeginArea(rect);
            EditorGUILayout.BeginHorizontal("toolbar");

            foreach (var w in Widgets)
            {
                w.OnGUi();
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            GUILayout.EndArea();
            Handles.EndGUI();
        }
    }
}