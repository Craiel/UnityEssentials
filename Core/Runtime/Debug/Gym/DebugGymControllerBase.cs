namespace Craiel.UnityEssentials.Runtime.Debug.Gym
{
    using Controllers;
    using UnityEngine;

    [RequireComponent(typeof(SphereCollider))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(FreeMovementController))]
    public class DebugGymControllerBase : MonoBehaviour
    {
        private SphereCollider colliderComponent;
        private Rigidbody rigidBodyComponent;
        private FreeMovementController movementController;
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Awake()
        {
            this.colliderComponent = this.GetComponent<SphereCollider>();
            
            this.rigidBodyComponent = this.GetComponent<Rigidbody>();
            this.rigidBodyComponent.useGravity = false;
            this.rigidBodyComponent.angularDrag = 0;
            
            this.movementController = this.GetComponent<FreeMovementController>();
        }
    }
}