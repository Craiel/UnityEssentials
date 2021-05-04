namespace Craiel.UnityEssentials.Editor
{
    using UnityEditor;

    public class SceneToolbarWidget
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool IsButtonWidget { get; protected set; }
        
        public virtual void Update()
        {
        }

        public virtual void PlaymodeStateChanged(PlayModeStateChange stateChange)
        {

        }

        public virtual void DrawGUI()
        {
            
        }
        
        public virtual void DrawSceneGUI(SceneView sceneView)
        {
        }
    }
}