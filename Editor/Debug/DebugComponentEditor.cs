namespace Craiel.UnityEssentials.Debug.Editor
{
    using UnityEditor;
    using UnityEssentials.Editor.UserInterface;

    public abstract class DebugComponentEditor : Editor
    {
        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected bool DrawFoldout(string title, ref bool toggle)
        {
            toggle = Layout.DrawSectionHeaderToggleWithSection(title, toggle);
            return toggle;
        }
    }
}