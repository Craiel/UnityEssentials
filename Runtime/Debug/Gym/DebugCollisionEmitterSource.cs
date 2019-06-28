namespace Craiel.UnityEssentials.Runtime.Debug.Gym
{
    using EngineCore.Jobs;
    using Pool;
    using Unity.Collections;
    using Unity.Jobs;
    using UnityEngine;
    using UnityEngine.Jobs;

    public class DebugCollisionEmitterSource : MonoBehaviour
    {
        private GameObjectPoolWithLifeTime objectPool;
        
        private float lastEmit;

        private NativeArray<Vector3> emissionVelocity;

        private JobHandle velocityJobHandle;
        private JobHandle positionJobHandle;
        
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
        public ushort MaxCount = 100;

        [SerializeField]
        public float AliveTime = 20f;

        public void Awake()
        {
            this.objectPool = new GameObjectPoolWithLifeTime(this.MaxCount, this.Prefab);
            this.objectPool.LifeTimeExpired += this.OnObjectExpired;
            
            this.emissionVelocity = new NativeArray<Vector3>(this.MaxCount, Allocator.Persistent);
        }

        public void Update()
        {
            this.UpdateEmit();
            
            this.objectPool.Update();

            var velocityJob = new GenericJobAccelerate
            {
                VelocityData = this.emissionVelocity,
                DeltaTime = Time.deltaTime,
                Acceleration = Vector3.one * 0.1f
            };
                
            var positionJob = new GenericJobUpdatePosition
            {
                VelocityData = this.emissionVelocity, 
                DeltaTime = Time.deltaTime
            };

            this.velocityJobHandle = velocityJob.Schedule(this.MaxCount, 64);
            this.positionJobHandle = positionJob.Schedule(this.objectPool.TransformAccess, this.velocityJobHandle);
        }

        public void LateUpdate()
        {
            this.positionJobHandle.Complete();
            
            this.objectPool.LateUpdate();
        }

        public void OnDestroy()
        {
            this.emissionVelocity.Dispose();
            this.objectPool.Dispose();
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void OnObjectExpired(int id)
        {
            this.emissionVelocity[id] = Vector3.zero;
        }
        
        private void UpdateEmit()
        {
            if (Time.time < this.lastEmit + this.Delay)
            {
                return;
            }

            if (!this.objectPool.Obtain(out GameObject instance, out ushort id))
            {
                return;
            }

            this.objectPool.SetLifeTime(id, this.AliveTime);
            
            instance.transform.position = this.transform.position;
            instance.transform.forward = this.transform.forward;
            
            this.lastEmit = Time.time;
        }
    }
}