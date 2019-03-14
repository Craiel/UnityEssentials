namespace Craiel.UnityEssentials.Runtime.Attributes
{
    using UnityEngine;

    public class HelpBoxAttribute : PropertyAttribute
    {
        public enum HelpBoxType
        {
            Info,
            Warning,
            Error
        }
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public HelpBoxAttribute(string message)
            : this(message, HelpBoxType.Info)
        {
        }

        public HelpBoxAttribute(string message, HelpBoxType type)
        {
            this.Message = message;
            this.Type = type;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Message { get; private set; }
        
        public HelpBoxType Type { get; private set; }
    }
}