namespace Assets.Scripts.Craiel.Essentials.Editor.NodeEditor
{
    using UnityEditor;
    using UnityEngine;

    public class ScriptableNodeStyleDefault : IScriptableNodeStyle
    {
        public static readonly ScriptableNodeStyleDefault Instance = new ScriptableNodeStyleDefault();
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ScriptableNodeStyleDefault()
        {
            this.Style = new GUIStyle();
            this.Style.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
            this.Style.border = new RectOffset(12, 12, 12, 12);
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public GUIStyle Style { get; private set; }
    }
}
