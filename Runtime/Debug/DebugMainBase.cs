namespace Craiel.UnityEssentials.Runtime.Debug
{
    using UnityEngine;

    public abstract class DebugMainBase : MonoBehaviour
    {
        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected T CreateComponent<T>()
            where T : DebugComponent
        {
            return this.CreateDebugSubNode(typeof(T).Name).AddComponent<T>();
        }

        protected GameObject CreateDebugSubNode(string nodeName)
        {
            GameObject result = new GameObject(nodeName);
            result.transform.SetParent(this.gameObject.transform);
            return result;
        }
    }
}
