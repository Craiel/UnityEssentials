namespace Craiel.UnityEssentials.Spatial
{
    public struct KDTreeTuple<TDimension, TNode>
    {
        public readonly TDimension Dimensions;

        public readonly TNode Node;

        public KDTreeTuple(TDimension dimensions, TNode node)
        {
            this.Dimensions = dimensions;
            this.Node = node;
        }
    }
}
