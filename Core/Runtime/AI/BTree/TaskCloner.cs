namespace Craiel.UnityEssentials.Runtime.AI.BTree
{
    using Contracts;
    using GDX.AI.Sharp.Runtime.BTree;

    /// <summary>
    /// Static holder of the Task Cloner settings
    /// </summary>
    public static class TaskCloner
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------

        /// <summary>
        /// Gets or sets the clone strategy (if any) that <see cref="Task{T}.Clone"/> will use. Defaults to null, meaning that <see cref="Task{T}.CopyTo"/> is used instead.
        /// In this case, properly overriding this method in each task is developer's responsibility
        /// </summary>
        public static ITaskCloner Current { get; set; }
    }
}
