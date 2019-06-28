namespace Craiel.UnityEssentials.Runtime.Pool
{
    using AI.BTree.Leafs;
    using Unity.Collections;
    using Unity.Jobs;
    using UnityEngine;

    public class GameObjectPoolWithLifeTime : GameObjectPool
    {
        private const float DefaultLifeTimeValue = 10f;
        
        private NativeArray<float> aliveTime;
        private NativeArray<float> lifeTimeRemaining;
        private NativeArray<bool> lifeTimeExceeded;
        
        private JobHandle lifeTimeJobHandle;
        
        private struct CheckAliveTimeJob : IJobParallelFor
        {
            [Unity.Collections.ReadOnly]
            public NativeArray<bool> Alive;
            
            public NativeArray<float> AliveTime;
            
            public NativeArray<float> LifeTimeRemaining;
            
            public NativeArray<bool> LifeTimeExceeded;

            public float DeltaTime;

            public void Execute(int i)
            {
                if (!this.Alive[i])
                {
                    return;
                }

                this.AliveTime[i] += this.DeltaTime;
                this.LifeTimeRemaining[i] -= this.DeltaTime;
                if (this.LifeTimeRemaining[i] <= 0)
                {
                    this.LifeTimeExceeded[i] = true;
                }
            }
        }

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public GameObjectPoolWithLifeTime(ushort objectCount, GameObject prefab) 
            : base(objectCount, prefab)
        {
            this.aliveTime = new NativeArray<float>(objectCount, Allocator.Persistent);
            this.lifeTimeRemaining = new NativeArray<float>(objectCount, Allocator.Persistent);
            this.lifeTimeExceeded = new NativeArray<bool>(objectCount, Allocator.Persistent);
            this.DefaultLifeTime = DefaultLifeTimeValue;
        }

        public GameObjectPoolWithLifeTime(GameObject[] instances) 
            : base(instances)
        {
            this.aliveTime = new NativeArray<float>(instances.Length, Allocator.Persistent);
            this.lifeTimeRemaining = new NativeArray<float>(instances.Length, Allocator.Persistent);
            this.lifeTimeExceeded = new NativeArray<bool>(instances.Length, Allocator.Persistent);
            this.DefaultLifeTime = DefaultLifeTimeValue;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public event System.Action<int> LifeTimeExpired;
        
        public float DefaultLifeTime { get; set; }

        public override bool Obtain(out GameObject entry, out ushort id)
        {
            if (base.Obtain(out entry, out id))
            {
                this.lifeTimeRemaining[id] = this.DefaultLifeTime;
                this.lifeTimeExceeded[id] = false;
                return true;
            }

            return false;
        }

        public void SetLifeTime(ushort id, float value)
        {
            if (!this.IsAlive(id))
            {
                return;
            }
            
            this.lifeTimeRemaining[id] = value;
        }

        public void Update()
        {
            var aliveTimeJob = new CheckAliveTimeJob
            {
                Alive = this.GetAliveState(),
                AliveTime = this.aliveTime,
                LifeTimeRemaining = this.lifeTimeRemaining,
                LifeTimeExceeded = this.lifeTimeExceeded,
                DeltaTime = Time.deltaTime
            };
            
            this.lifeTimeJobHandle = aliveTimeJob.Schedule(this.Count, 64);
        }

        public void LateUpdate()
        {
            this.lifeTimeJobHandle.Complete();
            
            for (ushort i = 0; i < this.lifeTimeExceeded.Length; i++)
            {
                if (this.lifeTimeExceeded[i])
                {
                    this.Release(i);
                    this.lifeTimeExceeded[i] = false;

                    this.LifeTimeExpired?.Invoke(i);
                }
            }
        }
        
        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            if (isDisposing)
            {
                this.aliveTime.Dispose();
            }
        }
    }
}