namespace Craiel.UnityEssentials.Runtime.AI.BTree
{
    using Contracts;
    using Enums;

    /// <summary>
    /// <see cref="LoopDecorator{T}"/> is an abstract class providing basic functionalities for concrete looping decorators
    /// </summary>
    /// <typeparam name="T">type of the blackboard object that tasks use to read or modify game state</typeparam>
    public abstract class LoopDecorator<T> : Decorator<T>
        where T : IBlackboard
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        protected LoopDecorator()
        {
        }

        protected LoopDecorator(TaskId child)
            : base(child)
        {
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------

        /// <summary>
        /// whether the <see cref="LoopDecorator{T}.Run"/> method must keep looping or not.
        /// true if it must keep looping; false otherwise
        /// </summary>
        public virtual bool Condition { get; protected set; }

        public override void Run()
        {
            Task<T> child = this.Stream.Get(this.Child);
            this.Condition = true;
            while (this.Condition)
            {
                if (child.Status == BTTaskStatus.Running)
                {
                    child.Run();
                }
                else
                {
                    child.SetControl(this.Id, this.Stream);
                    child.Start();
                    if (child.CheckGuard(this.Id))
                    {
                        child.Run();
                    }
                    else
                    {
                        child.Fail();
                    }
                }
            }
        }

        public override void ChildRunning(TaskId task, TaskId reporter)
        {
            base.ChildRunning(task, reporter);
            this.Condition = false;
        }
    }
}
