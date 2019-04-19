namespace Craiel.UnityEssentials.Runtime.AI.BTree
{
    using Contracts;

    public sealed class GuardEvaluator<T> : Task<T>
        where T : IBlackboard
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public GuardEvaluator()
        {
        }

        public GuardEvaluator(BehaviorStream<T> stream)
        {
            this.Stream = stream;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override int ChildCount
        {
            get { return 0; }
        }

        public override TaskId GetChild(int index)
        {
            return TaskId.Invalid;
        }

        public override void Run()
        {
        }

        public override void ChildSuccess(TaskId task)
        {
        }

        public override void ChildFail(TaskId task)
        {
        }

        public override void ChildRunning(TaskId task, TaskId reporter)
        {
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override int AddChildToTask(TaskId child)
        {
            return 0;
        }

        protected override void CopyTo(Task<T> clone)
        {
        }
    }
}
