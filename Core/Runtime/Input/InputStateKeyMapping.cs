namespace Craiel.UnityEssentials.Runtime.Input
{
    using System;
    using UnityEngine;

    public class InputStateKeyMapping : InputStateMapping
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public InputStateKeyMapping()
        {
            this.Keys = new KeyCode[0];
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------

        // For performance reasons we use public arrays and not properties
        public KeyCode[] Keys;
        
        public void AddKey(KeyCode value)
        {
            Array.Resize(ref this.Keys, this.Keys.Length + 1);
            this.Keys[this.Keys.Length - 1] = value;
        }
        
        public override void Update()
        {
            bool pressed = false;

            // This is used for joint mappings, we can have one or many axis triggering the same target (shared)
            for (var x = 0; x < this.Keys.Length; x++)
            {
                KeyCode key = this.Keys[x];
                pressed = Input.GetKey(key);
            }
            
            for (var s = 0; s < this.States.Length; s++)
            {
                InputStateEntry entry = this.States[s];
                this.UpdateState(0, pressed, entry.Control, entry.Mode);
            }
        }
    }
}
