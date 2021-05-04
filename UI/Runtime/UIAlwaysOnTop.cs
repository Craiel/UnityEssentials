namespace Craiel.UnityEssentialsUI.Runtime
{
    using System;
    using UnityEngine;

    [AddComponentMenu(EssentialCoreUI.ComponentMenuFolder + "/Always On Top")]
    public class UIAlwaysOnTop : MonoBehaviour, IComparable
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SerializeField] 
        private int Order;

        public int CompareTo(object other)
        {
            var typed = other as UIAlwaysOnTop;
            if (typed == null)
            {
                return 1;
            }

            return this.Order.CompareTo(typed.Order);
        }
    }
}