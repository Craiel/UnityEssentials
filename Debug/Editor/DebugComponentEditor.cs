namespace Assets.Scripts.Craiel.Essentials.Debug.Editor
{
    using Essentials.Editor.UserInterface;
    using UnityEditor;

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