namespace Craiel.UnityEssentials.Runtime.AI.BTree.Decorators
{
    using System;
    using BTree;
    using Contracts;
    using Exceptions;

    /// <summary>
    /// An <see cref="Include{T}"/> decorator grafts a subtree.
    /// When the sub-tree is grafted depends on the value of the <see cref="Lazy"/> attribute: at clone-time if is false, at run-time if is true
    /// </summary>
    /// <typeparam name="T">type of the blackboard object that tasks use to read or modify game state</typeparam>
    public class Include<T> : Decorator<T>
        where T : IBlackboard
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------

        /// <summary>
        /// Creates a non-lazy <see cref="Include{T}"/> decorator without specifying the sub-tree
        /// </summary>
        public Include()
        {
        }

        /// <summary>
        /// Creates an eager or lazy <see cref="Include{T}"/> decorator for the specified sub-tree
        /// </summary>
        /// <param name="subTree">the sub-tree reference, usually a path</param>
        /// <param name="lazy">whether inclusion should happen at clone-time (false) or at run-time (true)</param>
        public Include(string subTree, bool lazy = false)
        {
            this.SubTree = subTree;
            this.Lazy = lazy;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------

        /// <summary>
        /// Mandatory task attribute indicating the path of the sub-tree to include
        /// </summary>
        public string SubTree { get; set; }

        /// <summary>
        /// Optional task attribute indicating whether the sub-tree should be included at clone-time (false, the default) or at run-time (true)
        /// </summary>
        public bool Lazy { get; set; }

        /// <summary>
        /// The first call of this method lazily sets its child to the referenced sub-tree created through the <see cref="BehaviorTreeLibraryManager"/>.
        /// Subsequent calls do nothing since the child has already been set.
        /// </summary>
        /// <exception cref="NotSupportedException">if this <see cref="Include{T}"/> is eager</exception>
        public override void Start()
        {
            if (!this.Lazy)
            {
                throw new NotSupportedException(string.Format("A non-lazy {0} isn't meant to be run!", this.GetType().Name));
            }

            if (this.Child == TaskId.Invalid)
            {
                this.AddChild(this.CreateSubtreeRootTask());
            }

            base.Start();
        }

        /// <summary>
        /// Returns a clone of the referenced sub-tree if this <see cref="Include{T}"/> is eager; otherwise returns a clone of itself
        /// </summary>
        /// <returns>the cloned <see cref="Include{T}"/> task</returns>
        public override Task<T> Clone()
        {
            if (this.Lazy)
            {
                return base.Clone();
            }

            throw new NotImplementedException();
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------

        /// <summary>
        /// Copies this <see cref="Include{T}"/> to the given task. A {@link TaskCloneException} is thrown if this {@code Include} is eager
        /// </summary>
        /// <param name="clone">the task to be filled</param>
        /// <exception cref="TaskCloneException">if this <see cref="Include{T}"/> is eager</exception>
        protected override void CopyTo(Task<T> clone)
        {
            if (!this.Lazy)
            {
                throw new TaskCloneException(string.Format("A non-lazy {0} should never be copied", this.GetType().Name));
            }

            Include<T> include = (Include<T>)clone;
            include.SubTree = this.SubTree;
            include.Lazy = this.Lazy;
        }

        private TaskId CreateSubtreeRootTask()
        {
            // TODO: continue
            throw new NotImplementedException();
        }
    }
}
