namespace Craiel.UnityEssentials.Runtime.Attributes
{
    using UnityEngine;

    public class PopupAttribute : PropertyAttribute
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public PopupAttribute (params object[] entries)
        {
            this.Entries = entries;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public readonly object[] Entries;
    }
}