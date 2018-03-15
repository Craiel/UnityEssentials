namespace Craiel.UnityEssentials.Spatial
{
    using System;

    /// <summary>
    /// Allows one to navigate a binary tree stored in an <see cref="Array"/> using familiar
    /// tree navigation concepts.
    /// </summary>
    /// <typeparam name="TPoint">The type of the individual points.</typeparam>
    /// <typeparam name="TNode">The type of the individual nodes.</typeparam>
    public class KDTreeNavigator<TPoint, TNode>
    {
        /// <summary>
        /// A reference to the pointArray in which the binary tree is stored in.
        /// </summary>
        private readonly TPoint[] pointArray;

        private readonly TNode[] nodeArray;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="KDTreeNavigator{TPoint,TNode}"/> class.
        /// </summary>
        /// <param name="pointArray">The point array backing the binary tree.</param>
        /// <param name="nodeArray">The node array corresponding to the point array.</param>
        /// <param name="index">The index of the node of interest in the pointArray. If not given, the node navigator start at the 0 index (the root of the tree).</param>
        public KDTreeNavigator(TPoint[] pointArray, TNode[] nodeArray, int index = 0)
        {
            this.Index = index;
            this.pointArray = pointArray;
            this.nodeArray = nodeArray;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------

        /// <summary>
        /// The index in the pointArray that the current node resides in.
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// The left child of the current node.
        /// </summary>
        public KDTreeNavigator<TPoint, TNode> Left
        {
            get
            {
                return KDTreeNavigation.LeftChildIndex(this.Index) < this.pointArray.Length - 1
                    ? new KDTreeNavigator<TPoint, TNode>(this.pointArray, this.nodeArray,
                        KDTreeNavigation.LeftChildIndex(this.Index))
                    : null;
            }
        }

        /// <summary>
        /// The right child of the current node.
        /// </summary>
        public KDTreeNavigator<TPoint, TNode> Right
        {
            get
            {
                return KDTreeNavigation.RightChildIndex(this.Index) < this.pointArray.Length - 1
                    ? new KDTreeNavigator<TPoint, TNode>(this.pointArray, this.nodeArray,
                        KDTreeNavigation.RightChildIndex(this.Index))
                    : null;
            }
        }

        /// <summary>
        /// The parent of the current node.
        /// </summary>
        public KDTreeNavigator<TPoint, TNode> Parent
        {
            get
            {
                return this.Index == 0
                    ? null
                    : new KDTreeNavigator<TPoint, TNode>(this.pointArray, this.nodeArray,
                        KDTreeNavigation.ParentIndex(this.Index));
            }
        }

        /// <summary>
        /// The current <typeparamref name="TPoint"/>.
        /// </summary>
        public TPoint Point
        {
            get { return this.pointArray[this.Index]; }
        }

        /// <summary>
        /// The current <typeparamref name="TNode"/>
        /// </summary>
        public TNode Node
        {
            get { return this.nodeArray[this.Index]; }
        }
    }
}
