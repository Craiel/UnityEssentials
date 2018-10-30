namespace Craiel.UnityEssentials.Editor.Data
{
    using Runtime.Data;
    using UnityEditor;
    using UnityEngine;
    using UserInterface;

    [CustomPropertyDrawer(typeof(SerializableTimeSpan))]
    public class SerializableTimeSpanEditor : PropertyDrawer
    {
        private const int Digits = 4;
        private const float DigitWidth = 12;
        private const float DigitMargin = 18;
        private const float DigitLabelSize = 14;
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) 
        {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            Rect rect = new Rect(position.x, position.y, position.width, position.height);
            
            rect.width = DigitWidth * Digits;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative<SerializableTimeSpan>(x => x.Hours), new GUIContent(string.Empty, "Hours"));
                
            rect.x += DigitWidth * Digits;
            rect.width = DigitLabelSize;
            EditorGUI.LabelField(rect, "h");

            rect.x += DigitMargin;
            rect.width = DigitWidth * Digits;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative<SerializableTimeSpan>(x => x.Minutes), new GUIContent(string.Empty, "Minutes"));
                
            rect.x += DigitWidth * 2;
            rect.width = DigitLabelSize;
            EditorGUI.LabelField(rect, "m");

            rect.x += DigitMargin;
            rect.width = DigitWidth * Digits;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative<SerializableTimeSpan>(x => x.Seconds), new GUIContent(string.Empty, "Seconds"));
                
            rect.x += DigitWidth * Digits;
            rect.width = DigitLabelSize;
            EditorGUI.LabelField(rect, "s");
                
            rect.x += DigitMargin;
            rect.width = DigitWidth * (Digits + 1);
            EditorGUI.PropertyField(rect, property.FindPropertyRelative<SerializableTimeSpan>(x => x.Milliseconds), new GUIContent(string.Empty, "Milliseconds"));
            
            rect.x += DigitWidth * (Digits + 1);
            rect.width = DigitLabelSize;
            EditorGUI.LabelField(rect, "ms");

            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }
}