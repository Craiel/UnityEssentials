namespace Craiel.UnityEssentials.Editor.AttributeDrawers
{
    using System.Globalization;
    using Runtime.Attributes;
    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    [CanEditMultipleObjects]
    public class ReadOnlyAttributeDrawer : PropertyDrawer
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var typed = (ReadOnlyAttribute) this.attribute;
            if (!typed.ShowAsLAbel)
            {
                Rect propertyPosition = position;
                if (typed.CanCopy)
                {
                    propertyPosition.max -= new Vector2(60, 0);
                }
                
                EditorGUI.BeginDisabledGroup(true);
                EditorGUI.PropertyField(propertyPosition, property, label);
                EditorGUI.EndDisabledGroup();
            }
            else
            {
                this.DrawLabel(position, property, label);
            }

            if (typed.CanCopy && !property.hasMultipleDifferentValues)
            {
                position.x = position.xMax - 60;
                position.width = 40;
                if (GUI.Button(position, "Copy"))
                {
                    GUIUtility.systemCopyBuffer = property.stringValue;
                }
            }
        }
        
        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void DrawLabel(Rect position, SerializedProperty property, GUIContent label)
        {
            position.width -= 40;
            if (property.hasMultipleDifferentValues)
            {
                EditorGUI.LabelField(position, label.text, "-");
            }
            else
            {
                switch (property.propertyType)
                {
                    case SerializedPropertyType.String:
                    {
                        EditorGUI.LabelField(position, label.text, property.stringValue);
                        break;
                    }

                    case SerializedPropertyType.Integer:
                    {
                        EditorGUI.LabelField(position, label.text, property.intValue.ToString());
                        break;
                    }

                    case SerializedPropertyType.Float:
                    {
                        EditorGUI.LabelField(position, label.text, property.floatValue.ToString(CultureInfo.InvariantCulture));
                        break;
                    }

                    default:
                    {
                        UnityEngine.Debug.LogError("Unsupported format for ReadOnlyAttributeDrawer: " + property.propertyType);
                        break;
                    }
                }
            }
        }
    }
}