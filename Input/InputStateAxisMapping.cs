namespace Assets.Scripts.Craiel.Essentials.Input
{
    using System;
    using Enums;

    public class InputStateAxisMapping
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public InputStateAxisMapping()
        {
            this.Mode = InputAxisMode.Bidirectional;
            this.Axis = new string[0];
            this.States = new InputStateEntry[0];
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------

        // For performance reasons we use public arrays and not properties
        public string[] Axis;

        public InputStateEntry[] States;

        public InputAxisMode Mode { get; set; }

        public void AddAxis(string value)
        {
            Array.Resize(ref this.Axis, this.Axis.Length + 1);
            this.Axis[this.Axis.Length - 1] = value;
        }

        public void AddState(InputStateEntry value)
        {
            Array.Resize(ref this.States, this.States.Length + 1);
            this.States[this.States.Length - 1] = value;
        }
    }
}
