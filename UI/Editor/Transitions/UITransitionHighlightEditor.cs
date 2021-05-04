namespace Craiel.UnityEssentialsUI.Editor.Transitions
{
    using Runtime.Transitions;
    using UnityEditor;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(UITransitionHighlight))]
    public class UITransitionHighlightEditor : UITransitionBaseEditor
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public UITransitionHighlightEditor()
        {
            this.HasToggle = true;
            this.DrawAllColors = true;
        }
    }
}