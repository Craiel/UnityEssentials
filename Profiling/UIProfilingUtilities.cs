namespace Craiel.UnityEssentials.Profiling
{
    using UnityEngine;

    public static class UIProfilingUtilities
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static void LabelWithShadow(Rect rect, string s)
        {
            Color oldColor = GUI.color;
            GUI.color = Color.black;
            GUI.Label(new Rect(rect.x + 1, rect.y + 1, rect.width, rect.height), s);
            GUI.color = oldColor;
            GUI.Label(new Rect(rect.x, rect.y, rect.width, rect.height), s);
        }
    }
}
