namespace Craiel.UnityEssentials.Runtime.AI.BTree
{
    using System;
    using Contracts;
    using Exceptions;
    using GDX.AI.Sharp.Runtime.BTree;
    using Runtime.Exceptions;

    /// <summary>
    /// A <see cref="Decorator{T}"/> is a wrapper that provides custom behavior for its child. The child can be of any kind (branch task, leaf task, or another decorator)
    /// </summary>
    /// <typeparam name="T">type of the blackboard object that tasks use to read or modify game state</typeparam>
    public class Decorator<T> : Task<T>
        where T : IBlackboard
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------

        /// <summary>
        /// Creates a decorator with no child task
        /// </summary>
        public Decorator()
        {
        }

        /// <summary>
        /// Creates a decorator that wraps the given task
        /// </summary>
        /// <param name="child">the task that will be wrapped</param>
        public Decorator(TaskId child)
        {
            this.Child = child;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override int ChildCount
        {
            get { return this.Child == TaskId.Invalid ? 0 : 1; }
        }

        public override TaskId GetChild(int index)
        {
            if (index == 0 && this.Child != TaskId.Invalid)
            {
                return this.Child;
            }

            throw new IndexOutOfRangeException("index invalid or child not set");
        }

        public override void Run()
        {
            this.Stream.CurrentTaskToRun = new BehaviorStream<T>.BehaviorStreamTaskToRun(this.Child, this.Id);
        }

        public override void ChildRunning(TaskId task, TaskId reporter)
        {
            this.Running();
        }

        public override void ChildSuccess(TaskId task)
        {
            this.Success();
        }

        public override void ChildFail(TaskId task)
        {
            this.Fail();
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------

        /// <summary>
        /// Gets the child task wrapped by this decorator
        /// </summary>
        protected TaskId Child { get; private set; }

        protected override int AddChildToTask(TaskId child)
        {
            if (this.Child != TaskId.Invalid)
            {
                throw new IllegalStateException("A decorator task cannot have more than one child");
            }

            this.Child = child;
            return 0;
        }

        protected override void CopyTo(Task<T> clone)
        {
            if (this.Child != TaskId.Invalid)
            {
                Decorator<T> decorator = (Decorator<T>)clone;
                TaskId cloneId = decorator.Stream.Add(this.Stream.Get(this.Child).Clone());
                decorator.Child = cloneId;
            }
        }
    }
}
