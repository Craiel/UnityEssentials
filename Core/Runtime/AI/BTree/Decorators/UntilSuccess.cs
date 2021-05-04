namespace Craiel.UnityEssentials.Runtime.AI.BTree.Decorators
{
    using BTree;
    using Contracts;

    /// <summary>
    /// The <see cref="UntilSuccess{T}"/> decorator will repeat the wrapped task until that task succeeds, which makes the decorator succeed.
    /// <para></para>
    /// Notice that a wrapped task that always fails without entering the running status will cause an infinite loop in the current frame.
    /// </summary>
    /// <typeparam name="T">type of the blackboard object that tasks use to read or modify game state</typeparam>
    public class UntilSuccess<T> : LoopDecorator<T>
        where T : IBlackboard
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public UntilSuccess()
        {
        }

        public UntilSuccess(TaskId child)
            : base(child)
        {
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void ChildFail(TaskId task)
        {
            this.Condition = true;
        }

        public override void ChildSuccess(TaskId task)
        {
            this.Success();
            this.Condition = false;
        }
    }
}
