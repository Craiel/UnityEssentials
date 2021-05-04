namespace Craiel.UnityEssentials.Runtime.Input
{
    using System;
    using Contracts;
    using UnityEngine;

    public abstract class BaseInputState : IInputState
    {
        private InputStateMapping[] mappings;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        protected BaseInputState()
        {
            this.mappings = new InputStateMapping[0];
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool IsActive { get; private set; }

        public InputStateAxisMapping AddAxis(string axis)
        {
            var result = new InputStateAxisMapping();
            result.AddAxis(axis);

            Array.Resize(ref this.mappings, this.mappings.Length + 1);
            this.mappings[this.mappings.Length - 1] = result;

            return result;
        }

        public InputStateKeyMapping AddKey(KeyCode key)
        {
            var result = new InputStateKeyMapping();
            result.AddKey(key);

            Array.Resize(ref this.mappings, this.mappings.Length + 1);
            this.mappings[this.mappings.Length - 1] = result;

            return result;
        }

        public virtual void Activate()
        {
            // Default state we enable the cursor
            this.EnableCursor();

            this.IsActive = true;
        }

        public virtual void Deactivate()
        {
            this.IsActive = false;
        }
        
        public virtual void Update()
        {
            for (var i = 0; i < this.mappings.Length; i++)
            {
                this.mappings[i].Update();
            }
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected void EnableCursor()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        protected void DisableCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
