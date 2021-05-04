namespace Craiel.UnityEssentials.Editor.AttributeDrawers
{
    using System.Globalization;
    using Runtime.Attributes;
    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(ProgressBarAttribute))]
    public class ProgressBarAttributeDrawer : PropertyDrawer
    {
        private GUIStyle labelStyle;
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            if (this.labelStyle == null)
            {
                this.labelStyle = new GUIStyle("Label");
                this.labelStyle.alignment = TextAnchor.MiddleRight;
            }
            
            EditorGUI.showMixedValue = prop.hasMultipleDifferentValues;
            
            if (prop.propertyType == SerializedPropertyType.Integer)
            {
                this.DrawBar(position, prop.intValue, label);
            }
            else if (prop.propertyType == SerializedPropertyType.Float)
            {
                this.DrawBar(position, prop.floatValue, label);
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void DrawBar(Rect position, float value, GUIContent label)
        {
            var typed = (ProgressBarAttribute) this.attribute;
            
            var diff = typed.Max - typed.Min;
            var progress = value / diff;

            EditorGUI.PrefixLabel(position, label);
            position.x += EditorGUIUtility.labelWidth;
            position.width -= EditorGUIUtility.labelWidth;
            EditorGUI.ProgressBar(position, progress, value.ToString(CultureInfo.InvariantCulture));

            GUI.Label(position, typed.Min.ToString(CultureInfo.InvariantCulture));
            GUI.Label(position, typed.Max.ToString(CultureInfo.InvariantCulture), this.labelStyle);
        }
    }
}