namespace Craiel.UnityEssentials.Editor.Data
{
    using Runtime.Data;
    using UnityEditor;
    using UnityEngine;
    using UserInterface;

    [CustomPropertyDrawer(typeof(SerializableDateTime))]
    public class SerializableDateTimeEditor : PropertyDrawer
    {
        private const float DigitWidth = 12;
        private const float DigitMarginLarge = 18;
        private const float DigitMarginSmall = 8;
        private const float LabelSize = 50;
        private const float DigitLabelSizeLarge = 14;
        private const float DigitLabelSizeSmall = 14;
        private const float DateTimeMargin = 20;
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) 
        {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            bool drawDate = property.FindPropertyRelative<SerializableDateTime>(x => x.UseDate).boolValue;
            bool drawTime = property.FindPropertyRelative<SerializableDateTime>(x => x.UseTime).boolValue;

            Rect rect = new Rect(position.x, position.y, position.width, position.height);
            if (drawDate)
            {
                rect.width = LabelSize;
                EditorGUI.LabelField(rect, "Date: ");
                
                rect.x += LabelSize;
                rect.width = DigitWidth * 2;
                EditorGUI.PropertyField(rect, property.FindPropertyRelative<SerializableDateTime>(x => x.Day), new GUIContent(string.Empty, "Day (0 - 31)"));
                
                rect.x += DigitWidth * 2;
                rect.width = DigitLabelSizeLarge;
                EditorGUI.LabelField(rect, "D");

                rect.x += DigitMarginLarge;
                rect.width = DigitWidth * 2;
                EditorGUI.PropertyField(rect, property.FindPropertyRelative<SerializableDateTime>(x => x.Month), new GUIContent(string.Empty, "Month (0 - 12)"));
                
                rect.x += DigitWidth * 2 ;
                rect.width = DigitLabelSizeLarge;
                EditorGUI.LabelField(rect, "M");

                rect.x += DigitMarginLarge;
                rect.width = DigitWidth * 4;
                EditorGUI.PropertyField(rect, property.FindPropertyRelative<SerializableDateTime>(x => x.Year), new GUIContent(string.Empty, "Year"));

                rect.x += DigitWidth * 4;
                rect.width = DigitLabelSizeLarge;
                EditorGUI.LabelField(rect, "Y");
            }

            if (drawTime)
            {
                if (drawDate)
                {
                    rect.x += DateTimeMargin;
                }

                rect.width = LabelSize;
                EditorGUI.LabelField(rect, "Time: ");
                
                rect.x += LabelSize;
                rect.width = DigitWidth * 2;
                EditorGUI.PropertyField(rect, property.FindPropertyRelative<SerializableDateTime>(x => x.Hour), new GUIContent(string.Empty, "Hour (0 - 23)"));
                
                rect.x += DigitWidth * 2;
                rect.width = DigitLabelSizeSmall;
                EditorGUI.LabelField(rect, ":");

                rect.x += DigitMarginSmall;
                rect.width = DigitWidth * 2;
                EditorGUI.PropertyField(rect, property.FindPropertyRelative<SerializableDateTime>(x => x.Minute), new GUIContent(string.Empty, "Minute (0 - 59)"));
                
                rect.x += DigitWidth * 2;
                rect.width = DigitLabelSizeSmall;
                EditorGUI.LabelField(rect, ":");

                rect.x += DigitMarginSmall;
                rect.width = DigitWidth * 2;
                EditorGUI.PropertyField(rect, property.FindPropertyRelative<SerializableDateTime>(x => x.Second), new GUIContent(string.Empty, "Second (0 - 59)"));
                
                rect.x += DigitWidth * 2;
                rect.width = DigitLabelSizeSmall;
                EditorGUI.LabelField(rect, ":");
                
                rect.x += DigitMarginSmall;
                rect.width = DigitWidth * 3;
                EditorGUI.PropertyField(rect, property.FindPropertyRelative<SerializableDateTime>(x => x.Millisecond), new GUIContent(string.Empty, "Millisecond"));
            }

            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }
}