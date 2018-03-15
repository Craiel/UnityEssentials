namespace Craiel.UnityEssentials.Editor.NodeEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class ScriptableNodeGridLayouter : IScriptableNodeLayouter
    {
        private Rect drawArea;
        private readonly IList<IScriptableNode> nodeList;
        
        private Vector2 maxNodeSize;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ScriptableNodeGridLayouter()
        {
            this.nodeList = new List<IScriptableNode>();
        }
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public float ColumnMargin { get; set; }
        
        public float RowMargin { get; set; }
            
        public void BeginLayout(Rect drawArea)
        {
            this.drawArea = drawArea;
            
            this.maxNodeSize = Vector2.zero;
        }

        public void Layout(IScriptableNode node)
        {
            this.nodeList.Add(node);
            
            // Get the node size
            Vector2 size = node.GetSize();
            if (size.x > this.maxNodeSize.x)
            {
                this.maxNodeSize.x = size.x;
            }

            if (size.y > this.maxNodeSize.y)
            {
                this.maxNodeSize.y = size.y;
            }
        }

        public void EndLayout()
        {
            int columnCount = (int)Math.Floor(this.drawArea.width / (this.maxNodeSize.x + this.ColumnMargin));
            int currentRow = 0;
            int currentColumn = 0;
            foreach (IScriptableNode node in this.nodeList)
            {
                float offsetX = (this.maxNodeSize.x + this.ColumnMargin) * currentColumn;
                float offsetY = (this.maxNodeSize.y + this.RowMargin) * currentRow;
                node.Move(new Vector2(this.drawArea.x + offsetX, this.drawArea.y + offsetY));

                currentColumn++;
                if (currentColumn == columnCount)
                {
                    currentColumn = 0;
                    currentRow++;
                }
            }
            
            this.nodeList.Clear();
        }
    }
}