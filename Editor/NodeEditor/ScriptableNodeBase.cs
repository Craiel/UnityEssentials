namespace Assets.Scripts.Craiel.Essentials.Editor.NodeEditor
{
    using System;
    using GDX.AI.Sharp.Mathematics;
    using UnityEngine;

    public abstract class ScriptableNodeBase : IScriptableNode
    {
        private static readonly Vector2 DefaultSize = new Vector2(100, 30);

        private bool isBeingDragged;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        protected ScriptableNodeBase()
            : this(Vector2.zero, DefaultSize)
        {
        }

        protected ScriptableNodeBase(Vector2 startPosition, Vector2 initialSize)
        {
            this.EnableDrag = true;

            this.NodeRect = new Rect(startPosition, initialSize);
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool VisualChanged { get; set; }
        
        public virtual void Move(Vector2 position)
        {
            this.NodeRect.position = position;
        }

        public virtual void DragWorld(Vector2 delta)
        {
            this.NodeRect.position += delta;
        }
        
        public virtual void DragNode(Vector2 delta)
        {
            if (!this.EnableDrag)
            {
                return;
            }

            this.NodeRect.position += delta;
        }

        public virtual void Draw(Rect drawArea)
        {
            if (this.EnableConstrainToView)
            {
                this.ConstrainNodeToArea(drawArea);
            }
        }

        public Vector2 GetSize()
        {
            return this.NodeRect.size;
        }

        public virtual bool ProcessEvent(Event eventData)
        {
            switch (eventData.type)
            {
                case EventType.MouseDown:
                {
                    return this.ProcessEventMouseDown(eventData);
                }

                case EventType.MouseUp:
                {
                    return this.ProcessEventMouseUp(eventData);
                }

                case EventType.MouseDrag:
                {
                    return this.ProcessEventMouseDrag(eventData);
                }

                default:
                {
                    return false;
                }
            }
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected DynamicContextMenu ContextMenu;
        
        protected bool EnableDrag;

        protected bool EnableConstrainToView;

        protected Rect NodeRect;

        protected void SetSize(float width, float height)
        {
            if (Math.Abs(this.NodeRect.width - width) < MathUtils.Epsilon 
                && Math.Abs(this.NodeRect.height - height) < MathUtils.Epsilon)
            {
                return;
            }
            
            this.NodeRect.width = width;
            this.NodeRect.height = height;
            this.VisualChanged = true;
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void ConstrainNodeToArea(Rect area)
        {
            if (this.NodeRect.x < area.x)
            {
                this.NodeRect.x = area.x;
            }

            if (this.NodeRect.y < area.y)
            {
                this.NodeRect.y = area.y;
            }

            if (this.NodeRect.width < area.width || this.NodeRect.height < area.height)
            {
                // We won't fit in the window, nothing left to check
                return;
            }

            if (this.NodeRect.max.x > area.max.x)
            {
                this.NodeRect.x = area.max.x - this.NodeRect.width;
            }

            if (this.NodeRect.max.y > area.max.y)
            {
                this.NodeRect.y = area.max.y - this.NodeRect.height;
            }
        }

        private bool ProcessEventMouseDown(Event eventData)
        {
            switch (eventData.button)
            {
                case 0:
                {
                    if (this.EnableDrag && this.NodeRect.Contains(eventData.mousePosition))
                    {
                        this.isBeingDragged = true;
                    }

                    GUI.changed = true;
                    return true;
                }

                case 1:
                {
                    if (this.ContextMenu != null)
                    {
                        this.ContextMenu.Show(eventData.mousePosition);
                    }

                    return false;
                }

                default:
                {
                    return false;
                }
            }
        }

        private bool ProcessEventMouseUp(Event eventData)
        {
            this.isBeingDragged = false;
            return false;
        }

        private bool ProcessEventMouseDrag(Event eventData)
        {
            switch (eventData.button)
            {
                case 0:
                {
                    if (this.isBeingDragged)
                    {
                        this.DragNode(eventData.delta);
                        eventData.Use();
                        GUI.changed = true;
                        return true;
                    }

                    return false;
                }

                default:
                {
                    return false;
                }
            }
        }
    }
}
