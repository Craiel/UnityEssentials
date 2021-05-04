namespace Craiel.UnityEssentialsUI.Editor.Transitions
{
    using Runtime.Transitions;
    using UnityEditor;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(UITransitionEffect))]
    public class UITransitionEffectEditor : UITransitionBaseEditor
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public UITransitionEffectEditor()
        {
            this.HasToggle = true;
            this.DrawAllColors = true;
        }
    }
}