namespace Craiel.UnityEssentials.Debug.Gym
{
    using UnityEngine;

    public abstract class DebugGymBase : MonoBehaviour
    {
        private bool sceneBehaviorsDisabled;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SerializeField]
        public GameObject Controller;

        public void LateUpdate()
        {
            if (!this.sceneBehaviorsDisabled)
            {
                this.sceneBehaviorsDisabled = true;
                this.DisableSceneBehaviors();
            }
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected virtual void DisableSceneBehaviors()
        {
        }

        protected void DisableSceneBehavior<T>()
            where T : MonoBehaviour
        {
            var targets = FindObjectsOfType<T>();
            foreach (T target in targets)
            {
                target.enabled = false;
            }

            Debug.LogFormat("DEBUG Disabled {0} Behaviors of type {1}", targets.Length, typeof(T).Name);
        }
    }
}
