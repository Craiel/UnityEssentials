namespace Craiel.UnityEssentials.Runtime.Extensions
{
    using System.Collections.Generic;
    using UnityEngine;

    public static class GameObjectExtensions
    {
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
            IList<GameObject> childObjects = new List<GameObject>();
            foreach (Transform child in gameObject.transform)
            {
                childObjects.Add(child.gameObject);
            }

            foreach (GameObject childObject in childObjects)
            {
                childObject.transform.SetParent(target.transform);
            }
        }

        public static void ClearChildren(this GameObject gameObject)
        {
            foreach (Transform child in gameObject.transform)
            {
                Object.Destroy(child.gameObject);
            }
        }
    }
}
