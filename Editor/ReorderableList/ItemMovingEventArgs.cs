namespace Assets.Scripts.Craiel.Essentials.Editor.ReorderableList
{
    using System.ComponentModel;
    using Contracts;

    public sealed class ItemMovingEventArgs : CancelEventArgs
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ItemMovingEventArgs(IReorderableListAdaptor adaptor, int itemIndex, int destinationItemIndex)
        {
            this.Adaptor = adaptor;
            this.ItemIndex = itemIndex;
            this.DestinationItemIndex = destinationItemIndex;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public IReorderableListAdaptor Adaptor { get; private set; }

        public int ItemIndex { get; internal set; }

        public int DestinationItemIndex { get; internal set; }

        public int NewItemIndex
        {
            get
            {
                int result = this.DestinationItemIndex;
                if (result > this.ItemIndex)
                {
                    --result;
                }
                
                return result;
            }
        }
    }
}