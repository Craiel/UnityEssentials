namespace Craiel.UnityEssentials.Runtime.AI.BTree
{
    using System.Collections.Generic;
    using System.Linq;
    using Contracts;
    using Exceptions;
    using GDX.AI.Sharp.Runtime.BTree;
    using Runtime.Exceptions;

    /// <summary>
    /// A <see cref="SingleRunningBranchTask{T}"/> task is a branch task that supports only one running child at a time
    /// </summary>
    /// <typeparam name="T">type of the blackboard object that tasks use to read or modify game state</typeparam>
    public abstract class SingleRunningBranchTask<T> : BranchTask<T>
        where T : IBlackboard
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------

        /// <summary>
        /// Create a branch task with no children
        /// </summary>
        protected SingleRunningBranchTask()
        {
        }

        /// <summary>
        /// Create a branch task with a list of children
        /// </summary>
        /// <param name="children">list of this task's children, can be empty</param>
        protected SingleRunningBranchTask(IEnumerable<TaskId> children)
            : base(children)
        {
        }

        /// <summary>
        /// Create a branch task with a list of children
        /// </summary>
        /// <param name="children">parameter list of this task's children, can be empty</param>
        protected SingleRunningBranchTask(params TaskId[] children)
            : base(children)
        {
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void ChildRunning(TaskId task, TaskId reporter)
        {
            this.RunningChild = task;
            this.Running();
        }

        public override void ChildSuccess(TaskId task)
        {
            this.RunningChild = TaskId.Invalid;
        }

        public override void ChildFail(TaskId task)
        {
            this.RunningChild = TaskId.Invalid;
        }

        public override void Run()
        {
            if (this.RunningChild != TaskId.Invalid)
            {
                this.Stream.CurrentTaskToRun = new BehaviorStream<T>.BehaviorStreamTaskToRun(this.RunningChild, this.Id);
            }
            else
            {
                if (this.CurrentChildIndex >= this.Children.Count)
                {
                    throw new IllegalStateException("Should never happen; this case must be handled by subclasses in childXXX methods");
                }

                if (this.RandomChildren != null)
                {
                    int last = this.Children.Count - 1;
                    if (this.CurrentChildIndex < last)
                    {
                        // Random swap
                        int otherChildIndex = UnityEngine.Random.Range(this.CurrentChildIndex, last);
                        TaskId temp = this.RandomChildren[this.CurrentChildIndex];
                        this.RandomChildren[this.CurrentChildIndex] = this.RandomChildren[otherChildIndex];
                        this.RandomChildren[otherChildIndex] = temp;
                    }

                    this.RunningChild = this.RandomChildren[this.CurrentChildIndex];
                }
                else
                {
                    this.RunningChild = this.GetChild(this.CurrentChildIndex);
                }

                this.Stream.CurrentTaskToRun = new BehaviorStream<T>.BehaviorStreamTaskToRun(this.RunningChild, this.Id);
            }
        }

        public override void Start()
        {
            this.CurrentChildIndex = 0;
            this.RunningChild = TaskId.Invalid;
            base.Start();
        }

        public override void Reset()
        {
            base.Reset();

            this.CurrentChildIndex = 0;
            this.RunningChild = TaskId.Invalid;
            this.RandomChildren = null;
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------

        /// <summary>
        /// The child in the running status or null if no child is running
        /// </summary>
        protected TaskId RunningChild { get; set; }

        /// <summary>
        /// The index of the child currently processed
        /// </summary>
        protected int CurrentChildIndex { get; set; }

        /// <summary>
        /// Array of random children. If it's {@code null} this task is deterministic
        /// </summary>
        protected TaskId[] RandomChildren { get; set; }

        protected override void CancelRunningChildren(int startIndex)
        {
            base.CancelRunningChildren(startIndex);

            this.RunningChild = TaskId.Invalid;
        }

        protected override void CopyTo(Task<T> clone)
        {
            SingleRunningBranchTask<T> branch = (SingleRunningBranchTask<T>)clone;
            branch.RandomChildren = null;

            base.CopyTo(clone);
        }

        protected TaskId[] CreateRandomChildren()
        {
            return this.Children.ToArray();
        }
    }
}
