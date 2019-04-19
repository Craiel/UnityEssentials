namespace Craiel.UnityEssentials.Runtime.AI.BTree.Decorators
{
    using BTree;
    using Contracts;

    /// <summary>
    /// An <see cref="AlwaysSucceed{T}"/> decorator will succeed no matter the wrapped task succeeds or fails
    /// </summary>
    /// <typeparam name="T">type of the blackboard object that tasks use to read or modify game state</typeparam>
    public class AlwaysSucceed<T> : Decorator<T>
        where T : IBlackboard
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public AlwaysSucceed()
        {
        }
        
        public AlwaysSucceed(TaskId child)
            : base(child)
        {
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void ChildFail(TaskId task)
        {
            this.ChildSuccess(task);
        }
    }
}
