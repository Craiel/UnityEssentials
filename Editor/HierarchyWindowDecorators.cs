namespace Craiel.UnityEssentials.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Runtime;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;

    [InitializeOnLoad]
    internal class HierarchyWindowItemDecorators
    {
        private const bool DisplaySetActive = true;
        private const bool DisplayStaticFlags = false;
        private const bool DisplayComponents = true;
        private const int IconSize = 16;

        private static readonly HashSet<Component> TempComponents = new HashSet<Component>();

        private static readonly Type[] FilteredComponents =
        {
            TypeCache<Transform>.Value,
            TypeCache<RectTransform>.Value,
            TypeCache<CanvasScaler>.Value,
            TypeCache<CanvasRenderer>.Value,
            TypeCache<GraphicRaycaster>.Value,
            TypeCache<FlareLayer>.Value,
            TypeCache<ScriptableObject>.Value,
            TypeCache<ContentSizeFitter>.Value,
            TypeCache<LayoutElement>.Value,
            TypeCache<Shadow>.Value,
            TypeCache<MeshFilter>.Value
        };

        private static readonly HashSet<string> FilteredIcons = new HashSet<string>
        {
            "cs Script Icon"
        };

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        static HierarchyWindowItemDecorators()
        {
            EditorApplication.hierarchyWindowItemOnGUI -= OnItemGui;
            EditorApplication.hierarchyWindowItemOnGUI += OnItemGui;
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private static void OnItemGui(int instanceId, Rect selectionRect)
        {
            DrawIcon(instanceId, selectionRect);
        }

        private static void DrawIcon(int instanceId, Rect rect)
        {
            var gameObject = EditorUtility.InstanceIDToObject(instanceId) as GameObject;
            if (gameObject == null)
            {
                return;
            }

            rect.x += rect.width;
            rect.width = IconSize;

            if (DisplaySetActive)
            {
                rect.x -= IconSize;
                bool active = GUI.Toggle(rect, gameObject.activeSelf, string.Empty);
                if (active != gameObject.activeSelf)
                {
                    gameObject.SetActive(active);
                    EditorUtility.SetDirty(gameObject);
                    EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                }
            }

            if (DisplayComponents)
            {
                DrawComponents(instanceId, rect);
            }
        }

        private static void DrawComponents(int instanceId, Rect rect)
        {
            UpdateComponents(instanceId);
            if (TempComponents.Count == 0)
            {
                return;
            }

            var textures = new HashSet<Texture>();

            foreach (var component in TempComponents)
            {
                Texture2D previewTexture = AssetPreview.GetMiniThumbnail(component);
                if (previewTexture == null || FilteredIcons.Contains(previewTexture.name))
                {
                    continue;
                }

                textures.Add(previewTexture);
            }

            if (textures.Count > 0)
            {
                rect.x -= IconSize;
                GUI.Label(rect, " | ");
            }

            foreach (var texture in textures)
            {
                rect.x -= IconSize;
                GUI.DrawTexture(rect, texture);
            }
        }

        private static void UpdateComponents(int instanceId)
        {
            TempComponents.Clear();

            var sceneGameObject = EditorUtility.InstanceIDToObject(instanceId) as GameObject;
            if (sceneGameObject == null)
            {
                return;
            }

            var subComponents = sceneGameObject.GetComponents<Component>().ToList();
            foreach (Component component in subComponents)
            {
                if (component == null)
                {
                    continue;
                }

                var behavior = component as Behaviour;
                if (behavior != null && !behavior.enabled)
                {
                    continue;
                }

                Type componentType = component.GetType();
                bool isFiltered = false;
                foreach (var filteredComponent in FilteredComponents)
                {
                    if (componentType == filteredComponent)
                    {
                        isFiltered = true;
                        break;
                    }
                }

                if (isFiltered)
                {
                    continue;
                }

                TempComponents.Add(component);
            }
        }
    }
}