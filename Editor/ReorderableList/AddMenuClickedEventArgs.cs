namespace Assets.Scripts.Craiel.Essentials.Editor.ReorderableList
{
    using System;
    using Contracts;
    using UnityEngine;

    public delegate void AddMenuClickedEventHandler(object sender, AddMenuClickedEventArgs args);
    
    public sealed class AddMenuClickedEventArgs : EventArgs
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public AddMenuClickedEventArgs(IReorderableListAdaptor adaptor, Rect buttonPosition)
        {
            this.Adaptor = adaptor;
            this.ButtonPosition = buttonPosition;
        }
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public IReorderableListAdaptor Adaptor { get; private set; }

        public Rect ButtonPosition { get; internal set; }
    }
}