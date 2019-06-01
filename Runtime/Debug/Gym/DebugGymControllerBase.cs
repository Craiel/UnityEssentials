namespace Craiel.UnityEssentials.Runtime.Debug.Gym
{
    using Controllers;
    using UnityEngine;

    [RequireComponent(typeof(SphereCollider))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(FreeMovementController))]
    public class DebugGymControllerBase : MonoBehaviour
    {
        private SphereCollider collider;
        private Rigidbody rigidbody;
        private FreeMovementController movementController;
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Awake()
        {
            this.collider = this.GetComponent<SphereCollider>();
            
            this.rigidbody = this.GetComponent<Rigidbody>();
            this.rigidbody.useGravity = false;
            this.rigidbody.angularDrag = 0;
            
            this.movementController = this.GetComponent<FreeMovementController>();
        }
    }
}