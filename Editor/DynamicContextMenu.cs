using CollectionExtensions = Craiel.UnityEssentials.Extensions.CollectionExtensions;

namespace Craiel.UnityEssentials.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    public abstract class DynamicContextMenu : IDisposable
    {
        private readonly IList<ContextOperation> operations;
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        protected DynamicContextMenu()
        {
            this.operations = new List<ContextOperation>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Show(Vector2 mousePosition)
        {
            var genericMenu = new GenericMenu();

            foreach (ContextOperation operation in this.operations)
            {
                this.AddMenuEntry(genericMenu, operation);
            }

            genericMenu.ShowAsContext();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected void RegisterAction(string title, Action callback, int order = 0, string group = null)
        {
            var operation = new ContextOperation(order, title, group, callback);
            this.operations.Add(operation);
            this.ReorderContextMenu(); 
        }

        protected void UnregisterAction(Action callback)
        {
            ContextOperation target = this.operations.FirstOrDefault(x => x.Callback == callback);
            if (target != null)
            {
                this.operations.Remove(target);
                this.ReorderContextMenu();
            }
        }

        protected void Clear()
        {
            this.operations.Clear();
        }

        protected virtual void Dispose(bool isDisposing)
        {
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void ReorderContextMenu()
        {
            // No need to include group here, groups are being drawn indidual anyway
            IList<ContextOperation> ordered = this.operations.OrderBy(x => x.Group).ThenBy(x => x.Order).ToList();
            this.operations.Clear();
            CollectionExtensions.AddRange(this.operations, ordered);
        }
        
        private void AddMenuEntry(GenericMenu target, ContextOperation operation)
        {
            var content = string.IsNullOrEmpty(operation.Group) 
                ? new GUIContent(operation.Title)
                : new GUIContent(string.Format("{0}/{1}", operation.Group, operation.Title));
            
            target.AddItem(content, false, () => operation.Callback.Invoke());
        }

        private class ContextOperation
        {
            public ContextOperation(int order, string title, string group, Action callback)
            {
                this.Order = order;
                this.Title = title;
                this.Group = group;
                this.Callback = callback;
            }

            public readonly int Order;
            public readonly string Title;
            public readonly string Group;
            public readonly Action Callback;
        }
    }
}
