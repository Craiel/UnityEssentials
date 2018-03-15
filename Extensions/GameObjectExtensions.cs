namespace Craiel.UnityEssentials.Extensions
{
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
    }
}
