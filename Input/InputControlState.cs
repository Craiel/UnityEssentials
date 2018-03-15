namespace Craiel.UnityEssentials.Input
{
    using Enums;

    public class InputControlState
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public float Value { get; set; }

        public InputMappingState State { get; set; }

        public bool IsHeld
        {
            get
            {
                return (this.State & InputMappingState.Held) != 0;
            }
        }

        public bool IsDown
        {
            get
            {
                return (this.State & InputMappingState.Down) != 0;
            }
        }

        public bool IsUp
        {
            get
            {
                return (this.State & InputMappingState.Up) != 0;
            }
        }

        public void Reset()
        {
            this.Value = 0f;
        }

        public void ResetFull()
        {
            this.Value = 0;
            this.State = InputMappingState.None;
        }
    }
}