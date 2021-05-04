namespace Craiel.UnityEssentials.Runtime.EngineCore.Construct
{
    using Extensions;
    using UnityEngine;

    public static class ConstructLogic
    {
        public static readonly ConstructSettings Settings = new ConstructSettings();
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static ConstructSettings BeginConstruct()
        {
            return Settings;
        }
        
        public static void FinalizeLocalConstruct()
        {
            FinalizeConstructBase();
        }

        public static void FinalizeWorldConstruct()
        {
            FinalizeConstructBase();
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private static void FinalizeConstructBase()
        {
            // Re-parent all construct children to a new node that will be destroyed as part of the scene unload
            var sceneLayoutRoot = new GameObject("Scene");
            Settings.Root.ReparentChildrenTo(sceneLayoutRoot);
            
            // Get rid of the construct object
            Object.Destroy(Settings.Root);
            Settings.Root = null;
        }
    }
}