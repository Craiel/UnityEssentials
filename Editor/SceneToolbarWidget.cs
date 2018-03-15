namespace Craiel.UnityEssentials.Editor
{
    using UnityEditor;

    public class SceneToolbarWidget
    {

        public virtual void Update()
        {
        }

        public virtual void OnSceneGUI(SceneView sceneView)
        {
        }

        public virtual void PlaymodeStateChanged(PlayModeStateChange stateChange)
        {

        }

        public virtual void OnGUi()
        {
            
        }
    }
}