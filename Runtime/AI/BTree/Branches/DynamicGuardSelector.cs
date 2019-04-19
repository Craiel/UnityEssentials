namespace Craiel.UnityEssentials.Runtime.AI.BTree.Branches
{
    using System.Collections.Generic;
    using System.Linq;
    using BTree;
    using Contracts;

    /// <summary>
    /// A <see cref="DynamicGuardSelector{T}"/> is a branch task that executes the first child whose guard is evaluated to <code>false</code>. 
    /// At every AI cycle, the children's guards are re-evaluated, so if the guard of the running child is evaluated to <code>false</code>, 
    /// it is cancelled, and the child with the highest priority starts running.The <see cref="DynamicGuardSelector{T}"/>
    /// task finishes when no guard is evaluated to true (thus failing) or when its active child finishes(returning the active child's termination status)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DynamicGuardSelector<T> : BranchTask<T>
        where T : IBlackboard
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public DynamicGuardSelector() : this(new TaskId[0])
        {
        }

        public DynamicGuardSelector(IEnumerable<TaskId> children)
        {
            this.Children = children.ToList();
        }

        public DynamicGuardSelector(params TaskId[] children)
        {
            this.Children = children.ToList();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------

        /// <summary>
        /// The child in the running status or null if no child is running
        /// </summary>
        public TaskId RunningChild { get; protected set; }

        public override void ChildRunning(TaskId task, TaskId reporter)
        {
            this.RunningChild = task;
            this.Running();
        }

        public override void ChildSuccess(TaskId task)
        {
            this.RunningChild = TaskId.Invalid;
            this.Success();
        }

        public override void ChildFail(TaskId task)
        {
            this.RunningChild = TaskId.Invalid;
            this.Fail();
        }

        public override void Run()
        {
            TaskId childToRun = TaskId.Invalid;
            for (var i = 0; i < this.Children.Count; i++)
            {
                TaskId childId = this.GetChild(i);
                Task<T> child = this.Stream.Get(childId);
                if (child.CheckGuard(this.Id))
                {
                    childToRun = childId;
                    break;
                }
            }

            if (this.RunningChild != TaskId.Invalid && this.RunningChild != childToRun)
            {
                this.Stream.Get(this.RunningChild).Cancel();
                this.RunningChild = TaskId.Invalid;
            }

            if (childToRun == TaskId.Invalid)
            {
                this.Fail();
            }
            else
            {
                if (this.RunningChild == TaskId.Invalid)
                {
                    this.RunningChild = childToRun;

                    Task<T> child = this.Stream.Get(this.RunningChild);
                    child.SetControl(this.Id, this.Stream);
                    child.Start();
                }

                this.Stream.CurrentTaskToRun = new BehaviorStream<T>.BehaviorStreamTaskToRun(this.RunningChild, this.Id);
            }
        }

        public override void Reset()
        {
            base.Reset();
            this.RunningChild = TaskId.Invalid;
        }
    }
}
