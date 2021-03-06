﻿namespace Craiel.UnityEssentials.Runtime.Input
{
    public struct InputControl
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public InputControl(string id)
            : this()
        {
            this.Id = id;
        }
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Id { get; private set; }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(this.Id);
        }

        public override bool Equals(object obj)
        {
            return ((InputControl) obj).Id.Equals(this.Id);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
        
        public static bool operator ==(InputControl rhs, InputControl lhs)
        {
            return rhs.Id == lhs.Id;
        }

        public static bool operator !=(InputControl rhs, InputControl lhs)
        {
            return !(rhs == lhs);
        }
    }
}