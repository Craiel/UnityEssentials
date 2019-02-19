namespace Craiel.UnityEssentials.Runtime.EngineCore.Construct
{
    using UnityEngine;

    public class ConstructSettings
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public GameObject Root { get; set; }
        
        public void Clear()
        {
            if (this.Root != null)
            {
                Object.Destroy(this.Root);
                this.Root = null;
            }
        }
    }
}