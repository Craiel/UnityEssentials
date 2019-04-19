namespace Craiel.UnityEssentials.Runtime.AI.BTree.Exceptions
{
    using System;
    using GDX.AI.Sharp.Runtime.BTree;

    /// <summary>
    /// Thrown when invalid calls are made inside the <see cref="BehaviorTreeBuilder{T}"/>
    /// </summary>
    public class BehaviorTreeBuilderException : Exception
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public BehaviorTreeBuilderException()
        {
        }

        public BehaviorTreeBuilderException(string message)
            : base(message)
        {
        }

        public BehaviorTreeBuilderException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
