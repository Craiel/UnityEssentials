namespace Assets.Scripts.Craiel.Essentials.Editor.ReorderableList
{
    using System;
    using Contracts;

    public delegate void ItemInsertedEventHandler(object sender, ItemInsertedEventArgs args);
    
    public sealed class ItemInsertedEventArgs : EventArgs
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ItemInsertedEventArgs(IReorderableListAdaptor adaptor, int itemIndex, bool wasDuplicated)
        {
            this.Adaptor = adaptor;
            this.ItemIndex = itemIndex;
            this.WasDuplicated = wasDuplicated;
        }
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public IReorderableListAdaptor Adaptor { get; private set; }

        public int ItemIndex { get; private set; }

        public bool WasDuplicated { get; private set; }
    }
}