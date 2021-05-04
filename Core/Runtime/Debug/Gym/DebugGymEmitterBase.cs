namespace Craiel.UnityEssentials.Runtime.Debug.Gym
{
    using System;
    using Contracts;
    using Input;
    using Pool;
    using Resource;
    using UnityEngine;

    public abstract class DebugGymEmitterBase<T, TN> : DebugGymBase
        where T : MonoBehaviour, IPoolable
        where TN : GameObjectBehaviourPool<T>, new()
    {
        private readonly TN emitterPool;
        
        private bool isInitialized;

        private float lastEmitterTime;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        protected DebugGymEmitterBase()
        {
            this.emitterPool = new TN();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SerializeField]
        public float MinEmitterDelay = 1.0f;

        [SerializeField]
        public float EmitterVelocity = 1;

        [SerializeField]
        public int MaxEmitters = 30;

        public void Update()
        {
            if (!this.isInitialized)
            {
                return;
            }
            
            this.emitterPool.Update();

            if (InputHandler.Instance.GetControl(InputStateDebug.DebugConfirm).IsHeld)
            {
                if (Time.time > this.lastEmitterTime + this.MinEmitterDelay && this.emitterPool.ActiveCount < this.MaxEmitters)
                {
                    T emitter = this.emitterPool.Obtain();
                    this.OnEmit(emitter);
                    
                    this.lastEmitterTime = Time.time;
                }
            }
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected abstract void OnEmit(T instance);
        protected abstract bool EmitterUpdate(T instance);

        protected virtual void Initialize(ResourceKey emitterPrefabKey)
        {
            if (this.isInitialized)
            {
                throw new InvalidOperationException("Duplicate initialize call");
            }
            
            this.isInitialized = true;
            this.emitterPool.Initialize(emitterPrefabKey, this.EmitterUpdate);
        }
        
        protected virtual void Initialize(GameObject emitterPrefab)
        {
            if (this.isInitialized)
            {
                throw new InvalidOperationException("Duplicate initialize call");
            }
            
            this.isInitialized = true;
            this.emitterPool.Initialize(emitterPrefab, this.EmitterUpdate);
        }
    }
}
