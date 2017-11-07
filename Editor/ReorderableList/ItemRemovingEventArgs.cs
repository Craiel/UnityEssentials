namespace Assets.Scripts.Craiel.Essentials.Editor.ReorderableList
{
    using System.ComponentModel;
    using Contracts;

    public delegate void ItemRemovingEventHandler(object sender, ItemRemovingEventArgs args);
    
    public sealed class ItemRemovingEventArgs : CancelEventArgs
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ItemRemovingEventArgs(IReorderableListAdaptor adaptor, int itemIndex)
        {
            this.Adaptor = adaptor;
            this.ItemIndex = itemIndex;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public IReorderableListAdaptor Adaptor { get; private set; }

        public int ItemIndex { get; internal set; }
    }
}