namespace Craiel.UnityEssentials.Editor
{
    using System;
    using System.Collections.Generic;
    using Runtime;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;
    using UnityEngine.AI;
    using UnityEngine.SceneManagement;

    [InitializeOnLoad]
    internal class HierarchyWindowItemDecorators
    {
        private const bool DisplaySetActive = true;
        private const bool DisplayStaticFlags = false;
        private const bool DisplayComponents = true;
        private const int IconSize = 16;

        private static readonly Type[] FilteredComponents =
        {
            TypeCache<NavMeshObstacle>.Value,
            TypeCache<Collider>.Value,
            TypeCache<Camera>.Value,
            TypeCache<Light>.Value
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
            var components = GetComponents(instanceId);
            if (components == null)
            {
                return;
            }

            var textures = new HashSet<Texture>();

            foreach (var component in components)
            {
                textures.Add(AssetPreview.GetMiniThumbnail(component));
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

        private static HashSet<Component> GetComponents(int instanceId)
        {
            var components = new HashSet<Component>();
            var sceneGameObject = EditorUtility.InstanceIDToObject(instanceId) as GameObject;
            if (sceneGameObject == null)
            {
                return null;
            }

            foreach (var component in sceneGameObject.GetComponents<Component>())
            {
                if (component == null)
                {
                    continue;
                }

                Type componentType = component.GetType();
                foreach (var filteredComponent in FilteredComponents)
                {
                    if (componentType == filteredComponent || componentType.BaseType == filteredComponent)
                    {
                        components.Add(component);
                    }
                }
            }

            return components;
        }
    }
}