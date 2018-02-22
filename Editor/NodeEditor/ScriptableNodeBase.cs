namespace Assets.Scripts.Craiel.Essentials.Editor.NodeEditor
{
    using UnityEngine;

    public abstract class ScriptableNodeBase : IScriptableNode
    {
        private static readonly Vector2 DefaultSize = new Vector2(100, 30);

        private readonly DynamicContextMenu contextMenu;

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
            this.contextMenu = new DynamicContextMenu();

            this.EnableDrag = true;

            this.NodeRect = new Rect(startPosition, initialSize);
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public virtual void Move(Vector2 position)
        {
            this.NodeRect.position = position;
        }

        public virtual void Drag(Vector2 delta)
        {
            if (!this.EnableDrag)
            {
                return;
            }

            this.NodeRect.position += delta;
        }

        public virtual void Draw(Rect drawArea)
        {
            this.ConstrainNodeToArea(drawArea);
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
        protected bool EnableDrag;

        protected Rect NodeRect;

        protected void SetSize(int width, int height)
        {
            this.NodeRect.width = width;
            this.NodeRect.height = height;
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
                    this.contextMenu.Show(eventData.mousePosition);
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
                        this.Drag(eventData.delta);
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
