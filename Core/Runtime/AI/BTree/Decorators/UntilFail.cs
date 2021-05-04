namespace Craiel.UnityEssentials.Runtime.AI.BTree.Decorators
{
    using BTree;
    using Contracts;

    /// <summary>
    /// The <see cref="UntilFail{T}"/> decorator will repeat the wrapped task until that task fails, which makes the decorator succeed.
    /// <para></para>
    /// Notice that a wrapped task that always succeeds without entering the running status will cause an infinite loop in the current frame.
    /// </summary>
    /// <typeparam name="T">type of the blackboard object that tasks use to read or modify game state</typeparam>
    public class UntilFail<T> : LoopDecorator<T>
        where T : IBlackboard
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public UntilFail()
        {
        }

        public UntilFail(TaskId child)
            : base(child)
        {
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void ChildSuccess(TaskId task)
        {
            this.Condition = true;
        }

        public override void ChildFail(TaskId task)
        {
            this.Success();
            this.Condition = false;
        }
    }
}
