namespace Craiel.UnityEssentials.Runtime.AI.BTree.Leafs
{
    using BTree;
    using Contracts;
    using Enums;

    /// <summary>
    /// <see cref="Success{T}"/> is a leaf that immediately succeeds
    /// </summary>
    /// <typeparam name="T">type of the blackboard object that tasks use to read or modify game state</typeparam>
    public class Success<T> : LeafTask<T>
        where T : IBlackboard
    {
        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override BTTaskStatus Execute()
        {
            return BTTaskStatus.Succeeded;
        }

        protected override void CopyTo(Task<T> clone)
        {
        }
    }
}
