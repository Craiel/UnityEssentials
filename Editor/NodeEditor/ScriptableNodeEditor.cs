namespace Assets.Scripts.Craiel.Essentials.Editor.NodeEditor
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    public abstract class ScriptableNodeEditor
    {
        private readonly DynamicContextMenu generalContextMenu;

        private readonly IList<IScriptableNode> nodes;
        private readonly IList<IScriptableNodeConnection> connections;
        
        private Vector2 gridOffset;
        private Vector2 currentDrag;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        protected ScriptableNodeEditor()
        {
            this.generalContextMenu = new DynamicContextMenu();
            this.nodes = new List<IScriptableNode>();
            this.connections = new List<IScriptableNodeConnection>();
            
            this.BorderEnabled = true;
            this.BorderOpacity = 1f;
            this.BorderColor = Color.grey;

            this.GridEnableMeasureSections = true;
            this.GridSpacing = 20;
            this.GridSpacingMeasureMultiplier = 4;
            this.GridOpacity = 0.2f;
            this.GridOpacityMeasureMultiplier = 2;
            this.GridColor = Color.grey;
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected bool BorderEnabled;
        protected float BorderOpacity;
        protected Color BorderColor;

        protected bool GridEnableMeasureSections;
        protected float GridSpacing;
        protected float GridSpacingMeasureMultiplier;
        protected float GridOpacity;
        protected float GridOpacityMeasureMultiplier;
        protected Color GridColor;

        protected void Draw(Rect drawArea)
        {
            if (this.BorderEnabled)
            {
                this.DrawRect(drawArea, this.BorderOpacity, this.BorderColor);
            }
            
            this.DrawGrid(drawArea, this.GridSpacing, this.GridOpacity, this.GridColor);

            if (this.GridEnableMeasureSections)
            {
                this.DrawGrid(drawArea, 
                    this.GridSpacing * this.GridSpacingMeasureMultiplier, 
                    this.GridOpacity * this.GridOpacityMeasureMultiplier, 
                    this.GridColor);
            }

            foreach (IScriptableNode node in this.nodes)
            {
                node.Draw(drawArea);
            }

            foreach (IScriptableNodeConnection connection in this.connections)
            {
                connection.Draw();
            }
        }

        public bool ProcessEvent(Event eventData)
        {
            foreach (IScriptableNode node in this.nodes)
            {
                node.ProcessEvent(eventData);
            }

            switch (eventData.type)
            {
                case EventType.MouseDown:
                {
                    if (eventData.button == 1)
                    {
                        this.generalContextMenu.Show(eventData.mousePosition);
                        return true;
                    }

                    return false;
                }
                
                case EventType.MouseDrag:
                {
                    this.DragScreen(eventData.delta);
                    return true;
                }

                default:
                {
                    return false;
                }
            }
        }

        private void DragScreen(Vector2 delta)
        {
            this.gridOffset += delta;

            if (nodes != null)
            {
                for (int i = 0; i < nodes.Count; i++)
                {
                    nodes[i].Drag(delta);
                }
            }

            GUI.changed = true;
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected void Clear()
        {
            this.nodes.Clear();
            this.connections.Clear();
        }

        protected void RemoveNode(IScriptableNode node)
        {
            this.nodes.Remove(node);
        }

        protected void AddNode(IScriptableNode node)
        {
            this.nodes.Add(node);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void DrawRect(Rect drawArea, float opacity, Color color)
        {
            Handles.BeginGUI();
            Handles.color = new Color(color.r, color.g, color.b, opacity);

            Handles.DrawLine(drawArea.min, new Vector3(drawArea.min.x, drawArea.max.y));
            Handles.DrawLine(drawArea.min, new Vector3(drawArea.max.x, drawArea.min.y));

            Handles.DrawLine(drawArea.max, new Vector3(drawArea.min.x, drawArea.max.y));
            Handles.DrawLine(drawArea.max, new Vector3(drawArea.max.x, drawArea.min.y));

            Handles.color = Color.white;
            Handles.EndGUI();
        }

        private void DrawGrid(Rect drawArea, float spacing, float opacity, Color color)
        {
            int widthDivs = Mathf.CeilToInt(drawArea.width / spacing);
            int heightDivs = Mathf.CeilToInt(drawArea.height / spacing);

            Handles.BeginGUI();
            Handles.color = new Color(color.r, color.g, color.b, opacity);
            
            Vector3 offset = new Vector3(this.gridOffset.x % spacing, this.gridOffset.y % spacing, 0);
            for (int i = 0; i < widthDivs; i++)
            {
                float xOffset = drawArea.x + (spacing * i);
                Vector3 start = new Vector3(xOffset, drawArea.y, 0) + offset;
                Vector3 end = new Vector3(xOffset, drawArea.y + drawArea.height, 0f) + offset;
                Handles.DrawLine(start, end);
            }

            for (int j = 0; j < heightDivs; j++)
            {
                float yOffset = drawArea.y + (spacing * j);
                Vector3 start = new Vector3(drawArea.x, yOffset, 0) + offset;
                Vector3 end = new Vector3(drawArea.x + drawArea.width, yOffset, 0f) + offset;
                Handles.DrawLine(start, end);
            }

            Handles.color = Color.white;
            Handles.EndGUI();
        }
    }
}
