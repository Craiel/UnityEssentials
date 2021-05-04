namespace Craiel.UnityEssentials.Runtime.Debug.Gym
{
    using Contracts;
    using Pool;
    using UnityEngine;

    [RequireComponent(typeof(Rigidbody))]
    public class DebugCollisionEmitter : MonoBehaviour, IPoolable
    {
        private Rigidbody rigidBodyComponent;
        
        private Vector3 velocity;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SerializeField]
        public float Slowdown = 0.1f;

        [SerializeField]
        public float Lifetime = 4.0f;
        
        public float TimeOnInitialize { get; private set; }

        public void Initialize(Vector3 initialVelocity)
        {
            this.velocity = initialVelocity;
            this.gameObject.SetActive(true);

            this.TimeOnInitialize = Time.time;

            this.rigidBodyComponent = this.GetComponent<Rigidbody>();
        }

        public void Reset()
        {
            this.gameObject.SetActive(false);
        }

        public void Update()
        {
            this.rigidBodyComponent.MovePosition(this.transform.position + this.velocity);
            this.velocity *= 1.0f - (this.Slowdown * Time.deltaTime);
        }
    }
}