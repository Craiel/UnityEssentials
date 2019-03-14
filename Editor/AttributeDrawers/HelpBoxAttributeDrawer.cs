namespace Craiel.UnityEssentials.Editor.AttributeDrawers
{
    using Runtime.Attributes;
    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(HelpBoxAttribute))]
    public class HelpBoxDrawer : DecoratorDrawer
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override float GetHeight()
        {
            HelpBoxAttribute helpAttr = (HelpBoxAttribute) this.attribute;

            return Mathf.Max(40,GUI.skin.GetStyle("HelpBox").CalcHeight(new GUIContent(helpAttr.Message), EditorGUIUtility.currentViewWidth));
        }

        public override void OnGUI(Rect position)
        {
            HelpBoxAttribute attribute = (HelpBoxAttribute) this.attribute;

            MessageType type = MessageType.Info;
            switch (attribute.Type)
            {
                case HelpBoxAttribute.HelpBoxType.Warning:
                {
                    type = MessageType.Warning;
                    break;
                }

                case HelpBoxAttribute.HelpBoxType.Error:
                {
                    type = MessageType.Error;
                    break;
                }
            }
            
            EditorGUI.HelpBox(position, attribute.Message, type);
        }
    }
}