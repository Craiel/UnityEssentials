namespace Craiel.UnityEssentials.Runtime.AI.BTree.Exceptions
{
    using System;

    /// <summary>
    /// Exception thrown when serialization issues occur
    /// </summary>
    public class SerializationException : Exception
    {
        public SerializationException()
        {
        }

        public SerializationException(string message)
            : base(message)
        {
        }

        public SerializationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
