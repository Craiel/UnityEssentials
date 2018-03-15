namespace Craiel.UnityEssentials.Spatial
{
    /// <summary>
    /// Contains methods used for doing index arithmetic to traverse nodes in a binary tree.
    /// </summary>
    public static class KDTreeNavigation
    {
        /// <summary>
        /// Computes the index of the right child of the current node-index.
        /// </summary>
        /// <param name="index">The index of the current node.</param>
        /// <returns>The index of the right child.</returns>
        public static int RightChildIndex(int index)
        {
            return (2 * index) + 2;
        }

        /// <summary>
        /// Computes the index of the left child of the current node-index.
        /// </summary>
        /// <param name="index">The index of the current node.</param>
        /// <returns>The index of the left child.</returns>
        public static int LeftChildIndex(int index)
        {
            return (2 * index) + 1;
        }

        /// <summary>
        /// Computes the index of the parent of the current node-index.
        /// </summary>
        /// <param name="index">The index of the current node.</param>
        /// <returns>The index of the parent node.</returns>
        public static int ParentIndex(int index)
        {
            return (index - 1) / 2;
        }
    }
}
