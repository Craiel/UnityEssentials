namespace Assets.Scripts.Craiel.Essentials.Input
{
    using System;

    public class InputStateAxisMapping : InputStateMapping
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public InputStateAxisMapping()
        {
            this.Axis = new string[0];
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------

        // For performance reasons we use public arrays and not properties
        public string[] Axis;
        
        public InputStateAxisMapping AddAxis(string value)
        {
            Array.Resize(ref this.Axis, this.Axis.Length + 1);
            this.Axis[this.Axis.Length - 1] = value;
            return this;
        }

        public InputStateAxisMapping JoinWith(string axis)
        {
            this.AddAxis(axis);
            return this;
        }
        
        public override void Update()
        {
            float value = 0;
            bool pressed = false;

            // This is used for joint mappings, we can have one or many axis triggering the same target (shared)
            for (var x = 0; x < this.Axis.Length; x++)
            {
                string axi = this.Axis[x];
                float localValue = UnityEngine.Input.GetAxis(axi);

                if (Math.Abs(localValue) > float.Epsilon)
                {
                    value = localValue;
                    pressed = UnityEngine.Input.GetButton(axi);
                    break;
                }
            }

            for (var s = 0; s < this.States.Length; s++)
            {
                InputStateEntry entry = this.States[s];
                this.UpdateState(value, pressed, entry.Control, entry.Mode);
            }
        }
    }
}
