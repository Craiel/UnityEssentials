namespace Craiel.UnityEssentials.Runtime.AI.BTree
{
    using System.Collections.Generic;
    using System.Linq;
    using Contracts;

    /// <summary>
    /// A branch task defines a behavior tree branch, contains logic of starting or running sub-branches and leaves
    /// </summary>
    /// <typeparam name="T">type of the blackboard object that tasks use to read or modify game state</typeparam>
    public abstract class BranchTask<T> : Task<T>
        where T : IBlackboard
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------

        /// <summary>
        /// Create a branch task with no children
        /// </summary>
        protected BranchTask() : this(new TaskId[0])
        {
        }

        /// <summary>
        /// Create a branch task with a list of children
        /// </summary>
        /// <param name="children">list of this task's children, can be empty</param>
        protected BranchTask(IEnumerable<TaskId> children)
        {
            this.Children = children.ToList();
        }

        /// <summary>
        /// Create a branch task with a list of children
        /// </summary>
        /// <param name="children">parameter list of this task's children, can be empty</param>
        protected BranchTask(params TaskId[] children)
        {
            this.Children = children.ToList();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------

        /// <summary>
        /// The children of this branch task
        /// </summary>
        public IList<TaskId> Children { get; protected set; }

        public override int ChildCount
        {
            get { return this.Children.Count; }
        }

        public override TaskId GetChild(int index)
        {
            return this.Children[index];
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override int AddChildToTask(TaskId child)
        {
            this.Children.Add(child);
            return this.Children.Count - 1;
        }

        protected override void CopyTo(Task<T> clone)
        {
            BranchTask<T> branch = (BranchTask<T>)clone;
            if (this.Children != null)
            {
                for (var i = 0; i < this.Children.Count; i++)
                {
                    Task<T> child = this.Stream.Get(this.GetChild(i));
                    branch.AddChildToTask(branch.Stream.Add(child.Clone()));
                }
            }
        }
    }
}
