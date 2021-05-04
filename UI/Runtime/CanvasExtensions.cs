namespace Craiel.UnityEssentialsUI.Runtime
{
    using System;
    using UnityEngine;
    using UnityEssentials.Runtime.Extensions;

    public static class CanvasExtensions
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static void BringToFront(this GameObject gameObject, bool allowReparent = true)
        {
            // Use canvas as root
            Canvas canvas = gameObject.FindInParents<Canvas>();
            if (canvas == null)
            {
                EssentialCoreUI.Logger.Warn("BringToFront Failed, no target canvas found!");
                return;
            }
            
            canvas.BringToFront(gameObject, allowReparent);
        }
        
        public static void BringToFront(this Canvas target, GameObject gameObject, bool allowReparent = true)
        {
            Transform root = target.transform;

            // If the object has a parent canvas
            if (allowReparent && root != null)
            {
                gameObject.transform.SetParent(root, true);
            }

            // Set as last sibling
            gameObject.transform.SetAsLastSibling();

            // Handle the always on top components
            if (root != null)
            {
                UIAlwaysOnTop[] alwaysOnTopComponents = root.gameObject.GetComponentsInChildren<UIAlwaysOnTop>();

                if (alwaysOnTopComponents.Length > 0)
                {
                    // Sort them by order
                    Array.Sort(alwaysOnTopComponents);

                    foreach (UIAlwaysOnTop component in alwaysOnTopComponents)
                    {
                        component.transform.SetAsLastSibling();
                    }
                }
            }
        }
    }
}