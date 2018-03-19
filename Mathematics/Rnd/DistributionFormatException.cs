namespace Craiel.UnityEssentials.Mathematics.Rnd
{
    using System;

    /// <summary>
    /// Thrown to indicate that the application has attempted to convert a string to one of the distribution types, 
    /// but that the string does not have the appropriate format
    /// </summary>
    public class DistributionFormatException : Exception
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public DistributionFormatException()
        {
        }

        public DistributionFormatException(string message)
            : base(message)
        {
        }

        public DistributionFormatException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
