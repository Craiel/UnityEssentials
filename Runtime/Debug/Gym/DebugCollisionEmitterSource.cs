namespace Craiel.UnityEssentials.Runtime.Debug.Gym
{
    using System;
    using System.ComponentModel;
    using EngineCore.Jobs;
    using Pool;
    using Unity.Collections;
    using Unity.Jobs;
    using UnityEngine;
    using UnityEngine.Jobs;

    public class DebugCollisionEmitterSource : MonoBehaviour
    {
        private GameObjectTracker objectTracker;
        
        private float lastEmit;

        private GameObject prefabRoot;

        private NativeArray<Vector3> emissionVelocity;
        private NativeArray<bool> aliveTimeExceeded;

        private JobHandle velocityJobHandle;
        private JobHandle aliveTimeJobHandle;
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
            this.objectTracker = new GameObjectTracker(this.MaxCount);
            
            this.prefabRoot = new GameObject("EmitterRoot");
            
            this.emissionVelocity = new NativeArray<Vector3>(this.MaxCount, Allocator.Persistent);
            this.aliveTimeExceeded = new NativeArray<bool>(this.MaxCount, Allocator.Persistent);
        }
        
        private struct CheckAliveTimeJob : IJobParallelFor
        {
            [Unity.Collections.ReadOnly]
            public NativeArray<float> AliveTimes;

            public NativeArray<bool> AliveTimeExceeded;

            public float Time;

            public float MaxAliveTime;

            public void Execute(int i)
            {
                if (this.Time > this.AliveTimes[i] + this.MaxAliveTime)
                {
                    this.AliveTimeExceeded[i] = true;
                }
            }
        }

        public void Update()
        {
            this.UpdateEmit();

            var velocityJob = new GenericJobAccelerate
            {
                VelocityData = this.emissionVelocity,
                DeltaTime = Time.deltaTime,
                Acceleration = Vector3.one * 0.1f
            };

            var aliveTimeJob = new CheckAliveTimeJob
            {
                AliveTimes = this.objectTracker.GetAliveTimes(0),
                AliveTimeExceeded = this.aliveTimeExceeded,
                Time = Time.time,
                MaxAliveTime = this.AliveTime
            };
                
            var positionJob = new GenericJobUpdatePosition
            {
                VelocityData = this.emissionVelocity, 
                DeltaTime = Time.deltaTime
            };

            this.velocityJobHandle = velocityJob.Schedule(this.MaxCount, 64);
            this.aliveTimeJobHandle = aliveTimeJob.Schedule(this.MaxCount, 64, this.velocityJobHandle);
            this.positionJobHandle = positionJob.Schedule(this.objectTracker.GetTransformAccess(0), this.aliveTimeJobHandle);
        }

        public void LateUpdate()
        {
            this.positionJobHandle.Complete();

            for (var i = 0; i < this.aliveTimeExceeded.Length; i++)
            {
                if (this.aliveTimeExceeded[i])
                {
                    GameObjectTrackerTicket ticket = this.objectTracker.Get(i);
                    if (ticket != GameObjectTrackerTicket.Invalid)
                    {
                        this.objectTracker.Unregister(ref ticket);
                    }
                    
                    this.aliveTimeExceeded[i] = false;
                }
            }
        }

        public void OnDestroy()
        {
            this.emissionVelocity.Dispose();
            this.aliveTimeExceeded.Dispose();
            this.objectTracker.Dispose();
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void UpdateEmit()
        {
            if (Time.time < this.lastEmit + this.Delay)
            {
                return;
            }

            if (this.objectTracker.Entries >= this.MaxCount)
            {
                return;
            }

            GameObject instance = Instantiate(this.Prefab, this.prefabRoot.transform);
            instance.transform.position = this.transform.position;
            instance.transform.forward = this.transform.forward;
            
            instance.SetActive(true);
            
            this.objectTracker.Register(instance, out GameObjectTrackerTicket trackerTicket);
            this.objectTracker.Track(trackerTicket);
            
            this.lastEmit = Time.time;
        }
    }
}