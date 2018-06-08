namespace Craiel.UnityEssentials.Runtime.Logging
{
    using System;

    public class UnityStackTraceException : Exception
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public UnityStackTraceException(string stackTrace)
        {
            this.StackContents = stackTrace;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string StackContents { get; private set; }
    }
}
