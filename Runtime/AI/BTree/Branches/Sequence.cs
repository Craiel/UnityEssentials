namespace Craiel.UnityEssentials.Runtime.AI.BTree.Branches
{
    using System.Collections.Generic;
    using BTree;
    using Contracts;

    /// <summary>
    /// A <see cref="Sequence{T}"/> is a branch task that runs every children until one of them fails. If a child task succeeds, the selector will start and run the next child task
    /// </summary>
    /// <typeparam name="T">type of the blackboard object that tasks use to read or modify game state</typeparam>
    public class Sequence<T> : SingleRunningBranchTask<T>
        where T : IBlackboard
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public Sequence()
        {
        }

        public Sequence(IEnumerable<TaskId> children)
            : base(children)
        {
        }

        public Sequence(params TaskId[] children)
            : base(children)
        {
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void ChildSuccess(TaskId task)
        {
            base.ChildSuccess(task);

            if (++this.CurrentChildIndex < this.Children.Count)
            {
                // Set the next child to run
                this.RunningChild = this.Children[this.CurrentChildIndex];
                this.Stream.CurrentTaskToRun = new BehaviorStream<T>.BehaviorStreamTaskToRun(this.Id, this.Control);
                this.Running();
            }
            else
            {
                // All children processed, return success status
                this.Success();
            }
        }

        public override void ChildFail(TaskId task)
        {
            base.ChildFail(task);

            // Return failure status when a child says it failed
            this.Fail();
        }
    }
}
