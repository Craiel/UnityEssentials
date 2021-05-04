namespace Craiel.UnityEssentials.Runtime.Input
{
    using System;
    using Enums;

    public abstract class InputStateMapping
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        protected InputStateMapping()
        {
            this.States = new InputStateEntry[0];
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public InputStateEntry[] States;

        public abstract void Update();

        public InputStateMapping For(InputControl target, InputAxisMode axisMode = InputAxisMode.Bidirectional)
        {
            if (!target.IsValid() || !InputHandler.Instance.IsRegistered(target))
            {
                EssentialsCore.Logger.Error("Invalid target Control");
                return null;
            }

            var state = new InputStateEntry
            {
                Control = target,
                Mode = axisMode
            };

            this.AddState(state);
            return this;
        }

        public void AddState(InputStateEntry value)
        {
            Array.Resize(ref this.States, this.States.Length + 1);
            this.States[this.States.Length - 1] = value;
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected void UpdateState(float value, bool pressed, InputControl target, InputAxisMode mode)
        {
            InputControlState state = InputHandler.Instance.GetControl(target);
            SetState(state, pressed);
            SetValue(state, value, mode);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private static void SetState(InputControlState state, bool pressed)
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

        private static void SetValue(InputControlState state, float value, InputAxisMode mode)
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
