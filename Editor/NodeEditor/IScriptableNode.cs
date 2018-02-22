namespace Assets.Scripts.Craiel.Essentials.Editor.NodeEditor
{
    using UnityEngine;

    public interface IScriptableNode
    {
        void Move(Vector2 position);
        void Drag(Vector2 delta);
        void Draw(Rect drawArea);

        bool ProcessEvent(Event e);
    }
}
