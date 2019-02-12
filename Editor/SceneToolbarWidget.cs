namespace Craiel.UnityEssentials.Editor
{
    using UnityEditor;

    public class SceneToolbarWidget
    {

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