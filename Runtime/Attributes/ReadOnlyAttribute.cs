namespace Craiel.UnityEssentials.Runtime.Attributes
{
    using System;
    using UnityEngine;

    [AttributeUsage(AttributeTargets.Field)]
    public class ReadOnlyAttribute : PropertyAttribute
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ReadOnlyAttribute()
        {
            this.CanCopy = true;
            this.ShowAsLAbel = true;
        }
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool CanCopy { get; set; }
        
        public bool ShowAsLAbel { get; set; }
    }
}