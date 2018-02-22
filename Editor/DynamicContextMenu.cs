namespace Assets.Scripts.Craiel.Essentials.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    public class DynamicContextMenu
    {
        private readonly IList<ContextOperation> contextOperations;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public DynamicContextMenu()
        {
            this.contextOperations = new List<ContextOperation>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Show(Vector2 mousePosition)
        {
            GenericMenu genericMenu = new GenericMenu();
            foreach (ContextOperation operation in this.contextOperations)
            {
                var closure = operation;
                genericMenu.AddItem(operation.Content, false, () => closure.Callback.Invoke());
            }

            genericMenu.ShowAsContext();
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected void RegisterAction(string title, Action callback, int order = 0, int group = 0)
        {
            var operation = new ContextOperation(order, group, title, callback);
            this.contextOperations.Add(operation);
            this.ReorderContextMenu();
        }

        protected void UnregisterAction(Action callback)
        {
            ContextOperation operation = this.contextOperations.FirstOrDefault(x => x.Callback == callback);
            if (operation == null)
            {
                return;
            }

            this.contextOperations.Remove(operation);
            this.ReorderContextMenu();
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void ReorderContextMenu()
        {
            IList<ContextOperation> orderedOperations = this.contextOperations.OrderBy(x => x.Group).ThenBy(x => x.Order).ToList();
            this.contextOperations.Clear();
            this.contextOperations.AddRange(orderedOperations);
        }

        private class ContextOperation
        {
            public ContextOperation(int order, int group, string title, Action callback)
            {
                this.Order = order;
                this.Group = group;
                this.Content = new GUIContent(title);
                this.Callback = callback;
            }

            public readonly int Order;
            public readonly int Group;
            public readonly GUIContent Content;
            public readonly Action Callback;
        }
    }
}
