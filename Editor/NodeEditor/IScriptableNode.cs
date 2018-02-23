namespace Assets.Scripts.Craiel.Essentials.Editor.NodeEditor
{
    using UnityEngine;

    public interface IScriptableNode
    {
        bool VisualChanged { get; set; }
        
        void Move(Vector2 position);
        void DragWorld(Vector2 delta);
        void DragNode(Vector2 delta);
        void Draw(Rect drawArea);

        Vector2 GetSize();

        bool ProcessEvent(Event e);
    }
}
