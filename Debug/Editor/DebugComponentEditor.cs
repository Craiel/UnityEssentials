namespace Assets.Scripts.Craiel.Essentials.Debug.Editor
{
    using Essentials.Editor.UserInterface;
    using Logic.Stats;
    using UnityEditor;
    using UnityEngine;

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

        protected void DrawEntityStatInfo(EntityStatSet stats)
        {
            GUILayout.Label(string.Format("HP: {0}/{1}", stats.GetHealth(), stats.GetHealthMax()));

            GUILayout.Label(string.Format("SPD: {0}", stats.GetSpeed()));
            GUILayout.Label(string.Format("OFF: {0}", stats.GetOffense()));
            GUILayout.Label(string.Format("DEF: {0}", stats.GetDefense()));
            GUILayout.Label(string.Format("STA: {0}", stats.GetStamina()));
        }
    }
}