namespace Craiel.UnityEssentials.Runtime.AI.BTree.Enums
{
    /// <summary>
    /// The enumeration of the values that a task's status can have
    /// </summary>
    public enum BTTaskStatus
    {
        /// <summary>
        /// Means that the task has never run or has been reset
        /// </summary>
        Fresh,

        /// <summary>
        /// Means that the task needs to run again
        /// </summary>
        Running,

        /// <summary>
        /// Means that the task returned a failure result
        /// </summary>
        Succeeded,

        /// <summary>
        /// Means that the task returned a success result
        /// </summary>
        Failed,

        /// <summary>
        /// Means that the task has been terminated by an ancestor
        /// </summary>
        Canceled
    }
}
