namespace Craiel.UnityEssentials.Editor.UserInterface
{
    using UnityEditor;
    using UnityEngine;

    public static class Styles
    {
        private static Color? defaulEditortBackgroundColor;
        private static Color? defaulEditortTextColor;
        private static Color? defaulEditortSelectionColor;
        private static GUIStyle sectionHeader;
        private static GUIStyle sectionHeaderToggle;
        static GUIStyle sectionHeaderToggleArrow;
        static GUIStyle sectionContent;
        static GUIStyle subSectionContent;
        static GUIStyle titleStyle;
        static GUIStyle headerHorizontalStyle;
        static GUIStyle buttonIconStyle;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static Color DefaulEditortBackgroundColor
        {
            get
            {
                if (defaulEditortBackgroundColor == null)
                {
                    defaulEditortBackgroundColor = EditorGUIUtility.isProSkin
                        ? new Color32(56, 56, 56, 255)
                        : new Color32(194, 194, 194, 255);
                }

                return defaulEditortBackgroundColor.Value;
            }
        }
        
        public static Color DefaulEditortTextColor
        {
            get
            {
                if (defaulEditortTextColor.HasValue == false)
                {
                    defaulEditortTextColor = EditorGUIUtility.isProSkin
                        ? new Color32(180,180,180, 255)
                        : new Color32(0,0,0, 255);
                }
                return defaulEditortTextColor.Value;
            }
        }

        public static readonly Color DefaulEditortSelectedTextColor = new Color32(255, 255, 255, 255);
      
        public static Color DefaulEditortSelectionColor
        {
            get
            {
                if (defaulEditortSelectionColor == null)
                {
                    defaulEditortSelectionColor = EditorGUIUtility.isProSkin
                        ? new Color32(62, 95, 150, 255)
                        : new Color32(62, 125, 231, 255);
                }

                return defaulEditortSelectionColor.Value;
            }
        }
        
        public static GUIStyle SectionHeader
        {
            get
            {
                return sectionHeader ?? (sectionHeader = new GUIStyle("MiniToolbarButton")
                {
                    fontStyle = FontStyle.Bold,
                    fixedHeight = 25,
                    fontSize = 14,
                    alignment = TextAnchor.MiddleLeft
                });
            }
        }

        public static GUIStyle SectionHeaderToggle
        {
            get
            {
                return sectionHeaderToggle ?? (sectionHeaderToggle = new GUIStyle("MiniToolbarButton")
                {
                    padding = new RectOffset(20, 0, 0, 0),
                    fontStyle = FontStyle.Bold,
                    fixedHeight = 25,
                    fontSize = 12,
                    alignment = TextAnchor.MiddleLeft,
                    normal = {textColor = new Color32(218, 141, 2, 255)}
                });
            }
        }

        public static GUIStyle SectionHeaderToggleArrow
        {
            get
            {
                return sectionHeaderToggleArrow 
                    ?? (sectionHeaderToggleArrow = new GUIStyle("IN Foldout"));
            }
        }

        public static GUIStyle SectionContent
        {
            get
            {
                return sectionContent 
                    ?? (sectionContent = new GUIStyle("OL Box"));
            }
        }

        public static GUIStyle SubSectionContent
        {
            get
            {
                return subSectionContent ?? (subSectionContent = new GUIStyle("OL Box")
                {
                    margin = new RectOffset(5, 5, 5, 5),
                    stretchHeight = false
                });
            }
        }
        
        public static GUIStyle TitleStyle
        {
            get
            {
                return titleStyle 
                    ?? (titleStyle = new GUIStyle("PreToolbar") {alignment = TextAnchor.MiddleLeft});
            }
        }

        public static GUIStyle HeaderHorizontalStyle
        {
            get
            {
                return headerHorizontalStyle 
                    ?? (headerHorizontalStyle = new GUIStyle("PreToolbar"));
            }
        }

        public static GUIStyle ButtonIconStyle
        {
            get
            {
                return buttonIconStyle ?? (buttonIconStyle = new GUIStyle("button")
                {
                    fontStyle = FontStyle.Bold,
                    fixedHeight = 22,
                    alignment = TextAnchor.MiddleLeft
                });
            }
        }
    }
}
