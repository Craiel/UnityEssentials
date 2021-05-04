namespace Craiel.UnityEssentials.Runtime.AI.BTree.Branches
{
    using System.Collections.Generic;
    using BTree;
    using Contracts;

    /// <summary>
    /// A <see cref="Selector{T}"/> is a branch task that runs every children until one of them succeeds. 
    /// If a child task fails, the selector will start and run the next child task.
    /// </summary>
    /// <typeparam name="T">type of the blackboard object that tasks use to read or modify game state</typeparam>
    public class Selector<T> : SingleRunningBranchTask<T>
        where T : IBlackboard
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public Selector()
        {
        }

        public Selector(IEnumerable<TaskId> children)
            : base(children)
        {
        }

        public Selector(params TaskId[] children)
            : base(children)
        {
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void ChildFail(TaskId task)
        {
            base.ChildFail(task);
            if (++this.CurrentChildIndex < this.Children.Count)
            {
                // Run next child
                this.RunningChild = this.Children[this.CurrentChildIndex];
                this.Stream.CurrentTaskToRun = new BehaviorStream<T>.BehaviorStreamTaskToRun(this.Id, this.Control);
                this.Running();
            }
            else
            {
                // All children processed, return failure status
                this.Fail();
            }
        }

        public override void ChildSuccess(TaskId task)
        {
            base.ChildSuccess(task);

            // Return success status when a child says it succeeded
            this.Success();
        }
    }
}
