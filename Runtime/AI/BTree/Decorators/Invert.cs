namespace Craiel.UnityEssentials.Runtime.AI.BTree.Decorators
{
    using BTree;
    using Contracts;

    /// <summary>
    /// An <see cref="Invert{T}"/> decorator will succeed if the wrapped task fails and will fail if the wrapped task succeeds
    /// </summary>
    /// <typeparam name="T">type of the blackboard object that tasks use to read or modify game state</typeparam>
    public class Invert<T> : Decorator<T>
        where T : IBlackboard
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public Invert()
        {
        }
        
        public Invert(TaskId child)
            : base(child)
        {
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void ChildSuccess(TaskId task)
        {
            base.ChildFail(task);
        }

        public override void ChildFail(TaskId task)
        {
            base.ChildSuccess(task);
        }
    }
}
