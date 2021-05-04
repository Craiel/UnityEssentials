namespace Craiel.UnityEssentials.Editor
{
    using Runtime.Extensions;
    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(Color))]
    public class ColorPropertyDrawer : PropertyDrawer
    {
        private const float HexFieldWidth = 60f;
        private const float AlphaFieldWidth = 40f;
        private const float Spacing = 5f;
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
		
            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            float colorWidth = (position.width - HexFieldWidth - Spacing - AlphaFieldWidth - Spacing);

            Color32 color = property.colorValue;
            Color32 color2 = EditorGUI.ColorField(new Rect(position.x, position.y, colorWidth, position.height), property.colorValue);

            if (!color2.Equals(color))
            {
                property.colorValue = color = color2;
            }

            string colorString = EditorGUI.TextField(new Rect((position.x + colorWidth + Spacing), position.y, HexFieldWidth, position.height), color.ToHexString());
            try
            {
                color2 = colorString.ToColor();

                if (!color2.Equals(color))
                {
                    property.colorValue = color = color2;
                }
            }
            catch
            {
            }

            float newAlpha = EditorGUI.Slider(new Rect((position.x + colorWidth + HexFieldWidth + (Spacing * 2f)), position.y, AlphaFieldWidth, position.height), property.colorValue.a, 0f, 1f);

            if (!newAlpha.Equals(property.colorValue.a))
            {
                property.colorValue = new Color(property.colorValue.r, property.colorValue.g, property.colorValue.b, newAlpha);
            }
        
            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }
}