namespace Craiel.UnityEssentials.Runtime.AI.BTree.Contracts
{
    using BTree;
    using Enums;

    /// <summary>
    /// The listener interface for receiving task events. The class that is interested in processing a task event implements this
    ///  interface, and the object created with that class is registered with a behavior tree, using the
    ///  <see cref="BehaviorTree{T}.AddListener"/> method. When a task event occurs, the corresponding method is invoked.
    /// </summary>
    /// <typeparam name="T">type of the blackboard object that tasks use to read or modify game state</typeparam>
    public interface IListener<T>
        where T : IBlackboard
    {
        /// <summary>
        /// This method is invoked when the task status is set. This does not necessarily mean that the status has changed
        /// </summary>
        /// <param name="task">the task whose status has been set</param>
        /// <param name="previousStatus">the task's status before the update</param>
        void StatusUpdated(TaskId task, BTTaskStatus previousStatus);

        /// <summary>
        /// This method is invoked when a child task is added to the children of a parent task
        /// </summary>
        /// <param name="task">the parent task of the newly added child</param>
        /// <param name="index">the index where the child has been added</param>
        void ChildAdded(TaskId task, int index);
    }
}
