namespace Assets.Scripts.Craiel.Essentials.Editor.NodeEditor
{
    using UnityEngine;

    public interface IScriptableNodeLayouter
    {
        void BeginLayout(Rect drawRect);
        void Layout(IScriptableNode node);
        void EndLayout();
    }
}