namespace Craiel.UnityEssentials.Runtime.AI.BTree.Branches
{
    using System.Collections.Generic;
    using BTree;
    using Contracts;

    /// <summary>
    /// A <see cref="RandomSequence{T}"/> is a selector task's variant that runs its children in a random order
    /// </summary>
    /// <typeparam name="T">type of the blackboard object that tasks use to read or modify game state</typeparam>
    public class RandomSequence<T> : Sequence<T>
        where T : IBlackboard
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public RandomSequence()
        {
        }

        public RandomSequence(IEnumerable<TaskId> children)
            : base(children)
        {
        }

        public RandomSequence(params TaskId[] children)
            : base(children)
        {
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void Start()
        {
            base.Start();
            if (this.RandomChildren == null)
            {
                this.RandomChildren = this.CreateRandomChildren();
            }
        }
    }
}
