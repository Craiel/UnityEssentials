namespace Craiel.UnityEssentials.Runtime.Spatial
{
    using System;

    /// <summary>
    /// Contains extension methods for <see cref="BoundedPriorityList{TElement,TPriority}"/> class.
    /// </summary>
    public static class KDTreePriorityList
    {
        /// <summary>
        /// Takes a <see cref="BoundedPriorityList{TElement,TPriority}"/> storing the indexes of the points and nodes of a KDTree
        /// and returns the points and nodes.
        /// </summary>
        /// <param name="list">The <see cref="BoundedPriorityList{TElement,TPriority}"/>.</param>
        /// <param name="tree">The</param>
        /// <typeparam name="TPriority">THe type of the priority of the <see cref="BoundedPriorityList{TElement,TPriority}"/></typeparam>
        /// <typeparam name="TDimension">The type of the dimensions of the <see cref="Craiel.UnityEssentials.Spatial.KDTree{TDimension,TNode}"/></typeparam>
        /// <typeparam name="TNode">The type of the nodes of the <see cref="Craiel.UnityEssentials.Spatial.KDTree{TDimension,TNode}"/></typeparam>
        /// <returns>The points and nodes in the <see cref="Craiel.UnityEssentials.Spatial.KDTree{TDimension,TNode}"/> implicitly referenced by the <see cref="BoundedPriorityList{TElement,TPriority}"/>.</returns>
        public static KDTreeTuple<TDimension[], TNode>[] ToResultSet<TPriority, TDimension, TNode>(
           this KDTreePriorityList<int, TPriority> list,
           KDTree<TDimension, TNode> tree)
           where TDimension : IComparable<TDimension>
           where TPriority : IComparable<TPriority>
        {
            var array = new KDTreeTuple<TDimension[], TNode>[list.Count];
            for (var i = 0; i < list.Count; i++)
            {
                array[i] = new KDTreeTuple<TDimension[], TNode>(
                    tree.InternalPointArray[list[i]],
                    tree.InternalNodeArray[list[i]]);
            }

            return array;
        }
    }
}
