namespace Craiel.UnityEssentials.Debug.Gym
{
    using Contracts;
    using Input;
    using Pool;
    using Resource;
    using UnityEngine;

    public abstract class DebugGymEmitterBase<T, TN> : DebugGymBase
        where T : class, IPoolable
        where TN : GameObjectPool<T>, new()
    {
        private readonly TN emitterPool;

        private bool isLoaded;

        private float lastEmitterTime;

        private readonly ResourceKey emitterKey;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        protected DebugGymEmitterBase(ResourceKey emitterKey)
        {
            this.emitterPool = new TN();
            this.emitterKey = emitterKey;
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
            if (!this.isLoaded)
            {
                this.emitterPool.Initialize(this.emitterKey, this.EmitterUpdate);

                this.isLoaded = true;
            }

            this.emitterPool.Update();

            if (InputHandler.Instance.GetControl(InputStateDebug.DebugConfirm).IsHeld)
            {
                if (Time.time > this.lastEmitterTime + this.MinEmitterDelay && this.emitterPool.ActiveCount < this.MaxEmitters)
                {
                    // spawn a new emitter with current forward velocity
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
    }
}
