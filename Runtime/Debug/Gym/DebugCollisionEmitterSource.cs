namespace Craiel.UnityEssentials.Runtime.Debug.Gym
{
    using Pool;
    using UnityEngine;

    public class DebugCollisionEmitterSource : MonoBehaviour
    {
        private readonly GameObjectPool<DebugCollisionEmitter> emitterPool;
        
        private float lastEmit;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public DebugCollisionEmitterSource()
        {
            this.emitterPool = new GameObjectPool<DebugCollisionEmitter>();
        }
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SerializeField]
        public GameObject Prefab;
        
        [SerializeField]
        public float SpawnOffset;
        
        [SerializeField]
        public float Delay = 1f;

        [SerializeField]
        public float Velocity = 0.1f;

        [SerializeField]
        public float MaxCount = 100;

        public void Awake()
        {
            this.emitterPool.Initialize(this.Prefab, this.EmitterUpdate);
        }

        public void Update()
        {
            this.emitterPool.Update();
            
            if (Time.time < this.lastEmit + this.Delay)
            {
                return;
            }

            if (this.emitterPool.ActiveCount >= this.MaxCount)
            {
                return;
            }
         
            this.lastEmit = Time.time;
            DebugCollisionEmitter instance = this.emitterPool.Obtain();
            this.OnEmit(instance);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void OnEmit(DebugCollisionEmitter instance)
        {
            Vector3 localForward = this.transform.forward;
            
            instance.transform.position = this.transform.position + (localForward * this.SpawnOffset);
            instance.Initialize(localForward * this.Velocity);
        }

        private bool EmitterUpdate(DebugCollisionEmitter instance)
        {
            return !(Time.time > instance.TimeOnInitialize + instance.Lifetime);
        }
    }
}