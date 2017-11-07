namespace Assets.Scripts.Craiel.Essentials.Editor.ReorderableList
{
    using System;
    using Contracts;

    public delegate void ItemMovingEventHandler(object sender, ItemMovingEventArgs args);
    public delegate void ItemMovedEventHandler(object sender, ItemMovedEventArgs args);
    
    public sealed class ItemMovedEventArgs : EventArgs
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ItemMovedEventArgs(IReorderableListAdaptor adaptor, int oldItemIndex, int newItemIndex)
        {
            this.Adaptor = adaptor;
            this.OldItemIndex = oldItemIndex;
            this.NewItemIndex = newItemIndex;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public IReorderableListAdaptor Adaptor { get; private set; }

        public int OldItemIndex { get; internal set; }

        public int NewItemIndex { get; internal set; }
    }
}