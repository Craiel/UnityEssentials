namespace Craiel.UnityEssentials.Editor.AttributeDrawers
{
    using System;
    using Runtime.Attributes;
    using Runtime.Utils;
    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(PopupAttribute))]
    public class PopupAttributeDrawer : PropertyDrawer
    {
        private Type variableType;
        private Action<int> setValue;
        private Func<int, int> validateValue;
        private GUIContent[] displayValues;
        private object[] values;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var typed = (PopupAttribute) attribute;
            if (typed.Entries == null || typed.Entries.Length == 0)
            {
                EditorGUILayout.HelpBox("Popup has invalid values set!", MessageType.Error);
                return;
            }
            
            if (this.validateValue == null || this.setValue == null)
            {
                if (!this.Initialize(property))
                {
                    EditorGUILayout.HelpBox("Popup Initialize failed!", MessageType.Error);
                    return;
                }
            }

            int selectedIndex = 0;
            for (int i = 0; i < this.displayValues.Length; i++)
            {
                selectedIndex = this.validateValue(i);
                if (selectedIndex != 0)
                {
                    break;
                }
            }

            position.max -= new Vector2(20, 0);
            
            GUIContent customLabel = new GUIContent(label.text, string.Format("{0} (Value Type: {1})", label.tooltip ?? string.Empty, this.variableType.Name));
            
            EditorGUI.BeginChangeCheck();
            selectedIndex = EditorGUI.Popup(position, customLabel, selectedIndex, this.displayValues);
            if (EditorGUI.EndChangeCheck())
            {
                this.setValue(selectedIndex);
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private bool Initialize(SerializedProperty property)
        {
            var typed = (PopupAttribute) attribute;
            this.displayValues = new GUIContent[typed.Entries.Length];
            this.values = new object[typed.Entries.Length];
            for (int i = 0; i < typed.Entries.Length; i++)
            {
                this.displayValues[i] = new GUIContent(typed.Entries[i].ToString());
                this.values[i] = typed.Entries[i];
                
                if (this.variableType == null)
                {
                    this.variableType = typed.Entries[i].GetType();
                }
                else
                {
                    if (this.variableType != typed.Entries[i].GetType())
                    {
                        // Mismatched variable types
                        return false;
                    }
                }
            }

            string valueFormatString = "{0}";
            if (this.variableType == typeof(string))
            {
                this.validateValue = x =>
                {
                    return property.stringValue == (string)this.values[x] ? x : 0;
                };
                
                this.setValue = x =>
                {
                    property.stringValue = (string)this.values[x];
                };
            }
            else if (this.variableType == typeof(int))
            {
                this.validateValue = x =>
                {
                    return property.intValue == (int)this.values[x] ? x : 0;
                };
                
                this.setValue = x =>
                {
                    property.intValue = (int)this.values[x];
                };
            }
            else if (this.variableType == typeof(float))
            {
                this.validateValue = x =>
                {
                    return Math.Abs(property.floatValue - (float)this.values[x]) < EssentialMathUtils.Epsilon ? x : 0;
                };
                
                this.setValue = x =>
                {
                    property.floatValue = (float)this.values[x];
                };
                
                valueFormatString = "{0}f";
            }
            else
            {
                return false;
            }

            for (var i = 0; i < this.displayValues.Length; i++)
            {
                this.displayValues[i].text = string.Format(valueFormatString, this.displayValues[i].text);
            }

            return true;
        }
    }
}