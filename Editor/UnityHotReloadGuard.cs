namespace Craiel.UnityEssentials.Editor
{
    using Singletons;
    using UnityEditor;

    [InitializeOnLoad]
    public class UnityHotReloadGuard : UnitySingleton<UnityHotReloadGuard>
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        static UnityHotReloadGuard()
        {
            UnityEngine.Debug.Log("Enabling Hot-reload Guard");
            InstantiateAndInitialize();
        }

        ~UnityHotReloadGuard()
        {
            UnityEngine.Debug.Log("Disabling Hot-reload Guard");
            DestroyInstance();
        }

        public override void Initialize()
        {
            base.Initialize();

            EditorApplication.update += this.OnEditorUpdate;
        }

        public override void DestroySingleton()
        {
            EditorApplication.update -= this.OnEditorUpdate;

            base.DestroySingleton();
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void OnEditorUpdate()
        {
            if (EditorApplication.isPlaying && EditorApplication.isCompiling)
            {
                UnityEngine.Debug.Log("Exiting play mode due to script compilation.");
                EditorApplication.isPlaying = false;
            }
        }
    }
}
