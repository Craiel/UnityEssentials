namespace Craiel.UnityEssentials.Runtime.Extensions
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public static class GameObjectExtensions
    {
        private static readonly List<GameObject> GameObjectTempList = new List<GameObject>();
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static T GetOrAddComponent<T>(this GameObject gameObject)
            where T : UnityEngine.Component
        {
            T component = gameObject.GetComponent<T>();
            if (component != null)
            {
                return component;
            }

            return gameObject.AddComponent<T>();
        }

        public static void ReparentChildrenTo(this GameObject gameObject, GameObject target)
        {
            GameObjectTempList.Clear();
            foreach (Transform child in gameObject.transform)
            {
                GameObjectTempList.Add(child.gameObject);
            }

            foreach (GameObject childObject in GameObjectTempList)
            {
                childObject.transform.SetParent(target.transform);
            }
        }

        public static void ClearChildren(this GameObject gameObject)
        {
            GameObjectTempList.Clear();
            foreach (Transform child in gameObject.transform)
            {
                GameObjectTempList.Add(child.gameObject);
            }

            foreach (GameObject child in GameObjectTempList)
            {
                Object.Destroy(child);
            }
        }
        
        public static bool IsInLayer(this GameObject target, string layerName)
        {
            return target.layer == LayerMask.NameToLayer(layerName);
        }
        
        public static GameObject CreateEmptyGameObject(string name = null, Transform parent = null, params System.Type[] components)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = "New Game Object";
            }

            var result = new GameObject(name, components);
            result.transform.SetParent(parent);
            result.transform.localPosition = Vector3.zero;

            return result;
        }
        
        public static T FindInParents<T>(this GameObject gameObject) 
            where T : Component
        {
            if (gameObject == null)
            {
                return null;
            }
			
            var candidate = gameObject.GetComponent<T>();

            if (candidate != null)
            {
                return candidate;
            }
			
            Transform parent = gameObject.transform.parent;
			
            while (parent != null && candidate == null)
            {
                candidate = parent.gameObject.GetComponent<T>();
                parent = parent.parent;
            }
			
            return candidate;
        }
        
        public static GameObject FindGameObject(string name, GameObject root = null)
        {
            if (root)
            {
                if (root.name == name)
                {
                    return root;
                }

                foreach (Transform child in root.transform)
                {
                    GameObject result = FindGameObject(name, child.gameObject);
                    if (result != null)
                    {
                        return result;
                    }
                }

                return null;
            }
            
            Scene scene = SceneManager.GetActiveScene();
            GameObjectTempList.Clear();
            scene.GetRootGameObjects(GameObjectTempList);
            foreach (var candidate in GameObjectTempList)
            {
                GameObject result = FindGameObject(name, candidate);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
    }
}
