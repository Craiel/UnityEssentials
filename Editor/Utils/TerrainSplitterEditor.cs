namespace Craiel.UnityEssentials.Editor.Utils
{
    using Runtime.Utils;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(TerrainSplitter))]
    public class TerrainSplitterEditor : EssentialEditorIM
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var typedTarget = (TerrainSplitter)this.target;
            if (GUILayout.Button("Split"))
            {
                typedTarget.Split();
            }
        }
    }
}