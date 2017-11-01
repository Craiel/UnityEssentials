namespace Assets.Scripts.Craiel.Essentials.Input
{
    using System;
    using Contracts;
    using Enums;
    using UnityEngine;

    public abstract class BaseInputState : IInputState
    {
        private InputStateAxisMapping[] mappings;

        private InputStateAxisMapping currentEditedMapping;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        protected BaseInputState()
        {
            this.mappings = new InputStateAxisMapping[0];
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool IsActive { get; private set; }

        public IInputState AddAxis(string axis)
        {
            Array.Resize(ref this.mappings, this.mappings.Length + 1);
            this.currentEditedMapping = new InputStateAxisMapping();
            this.currentEditedMapping.AddAxis(axis);
            this.mappings[this.mappings.Length - 1] = this.currentEditedMapping;

            return this;
        }

        public IInputState JoinWith(string axis)
        {
            this.currentEditedMapping.AddAxis(axis);
            return this;
        }

        public IInputState WithMode(InputAxisMode mode)
        {
            this.currentEditedMapping.Mode = mode;
            return this;
        }

        public IInputState For(object target)
        {
            var state = new InputStateEntry { Control = target, Mode = this.currentEditedMapping.Mode };
            this.currentEditedMapping.AddState(state);
            return this;
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
                float value = 0;
                bool pressed = false;

                // This is used for joint mappings, we can have one or many axis triggering the same target (shared)
                InputStateAxisMapping mapping = this.mappings[i];
                for (var x = 0; x < mapping.Axis.Length; x++)
                {
                    string axi = mapping.Axis[x];
                    float localValue = Input.GetAxis(axi);

                    if (Math.Abs(localValue) > float.Epsilon)
                    {
                        value = localValue;
                        pressed = Input.GetButton(axi);
                        break;
                    }
                }

                for (var s = 0; s < mapping.States.Length; s++)
                {
                    InputStateEntry entry = mapping.States[s];
                    this.UpdateState(value, pressed, entry.Control, entry.Mode);
                }
            }
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected void UpdateState(float value, bool pressed, object target, InputAxisMode mode)
        {
            InputControlState state = InputHandler.Instance.GetControl(target);
            this.SetState(state, pressed);
            this.SetValue(state, value, mode);
        }

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

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void SetState(InputControlState state, bool pressed)
        {
            // First see what state we are in now, then we will go from there
            if (pressed)
            {
                if ((state.State & InputMappingState.Held) == 0)
                {
                    // We did not previously hold this button either so nothing has changed
                    if ((state.State & InputMappingState.Down) == 0)
                    {
                        // We changed to pressed
                        state.State |= InputMappingState.Down;
                        state.State &= ~InputMappingState.Up;
                    }
                }
                else
                {
                    // Remove the down flag since we keep on holding the button
                    state.State &= ~InputMappingState.Down;
                }

                // Last we set the actual held state
                state.State |= InputMappingState.Held;
            }
            else
            {
                if ((state.State & InputMappingState.Held) != 0)
                {
                    // We held the button before so now it's the up state
                    if ((state.State & InputMappingState.Up) == 0)
                    {
                        // We changed to un-pressed
                        state.State |= InputMappingState.Up;
                        state.State &= ~InputMappingState.Down;
                    }
                }
                else
                {
                    // Remove the up flag since we no longer held the button
                    state.State &= ~InputMappingState.Up;
                }

                // Last we set the actual held state
                state.State &= ~InputMappingState.Held;
            }
        }

        private void SetValue(InputControlState state, float value, InputAxisMode mode)
        {
            if (Math.Abs(value) < float.Epsilon)
            {
                // Nothing do do
                return;
            }

            if ((mode == InputAxisMode.Positive && value < 0) || (mode == InputAxisMode.Negative && value > 0))
            {
                // The value is invalid for the requirements of the mapping
                return;
            }

            state.Value = value;
        }
    }
}
