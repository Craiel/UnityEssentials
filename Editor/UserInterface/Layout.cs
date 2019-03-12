namespace Craiel.UnityEssentials.Editor.UserInterface
{
    using UnityEditor;
    using UnityEngine;

    public static class Layout
    {
        public enum LayoutIcon
        {
            Add,
            Remove
        }

        private static Color backgroundColorTemp;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static void DrawSectionHeader(string header)
        {
            GUILayout.Label(header, Styles.SectionHeader);
        }

        public static bool DrawSectionHeaderToggle(string header, bool value)
        {
            var val = GUILayout.Toggle(value, header, Styles.SectionHeaderToggle);
            var rect = GUILayoutUtility.GetLastRect();
            rect.x += 3;
            rect.y += 3;
            GUI.Toggle(rect, val, string.Empty, Styles.SectionHeaderToggleArrow);
            return val;
        }
        
        public static bool DrawSectionHeaderToggleWithSection(string header, bool value)
        {
            var val = GUILayout.Toggle(value, header, Styles.SectionHeaderToggle);
            var rect = GUILayoutUtility.GetLastRect();
            rect.x += 3;
            rect.y += 3;
            GUI.Toggle(rect, val, string.Empty, Styles.SectionHeaderToggleArrow);
            return val;
        }
        
        public static void BeginSectionContent(params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginVertical(Styles.SectionContent, options);
        }

        public static void EndSectionContent()
        {
            EditorGUILayout.EndVertical();
        }
        
        public static void BeginHeaderHorizontal(params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal(Styles.HeaderHorizontalStyle, options);
        }

        public static void EndHeaderHorizontal()
        {
            EditorGUILayout.EndHorizontal();
        }
        
        public static void BeginSubSectionContent(string title = "", params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginVertical(Styles.SubSectionContent, options);
            if (!string.IsNullOrEmpty(title))
            {
                Title(title);
                GUILayout.Space(5);
            }
        }

        public static void EndSubSectionContent()
        {
            GUILayout.Space(5);
            EditorGUILayout.EndVertical();
            GUILayout.Space(5);
        }

        public static Rect BeginVerticalBox()
        {
            return EditorGUILayout.BeginVertical("box");
        }

        public static void EndVerticalBox()
        {
            EditorGUILayout.EndVertical();
        }

        public static bool MiniButton(string title, params GUILayoutOption[] options)
        {
            return GUILayout.Button(title, EditorStyles.miniButton, options);
        }

        public static bool Button(string title, params GUILayoutOption[] options)
        {
            return GUILayout.Button(title, options);
        }

        public static void Title(string title, float space = 0)
        {
            GUILayout.Space(space);
            GUILayout.Label(title, Styles.TitleStyle);
            GUILayout.Space(space);
        }
        
        public static bool ButtonIcon(string title, LayoutIcon icon, params GUILayoutOption[] options)
        {
            Texture iconTexture = EditorGUIUtility.Load(string.Format("Common/{0}.png", icon)) as Texture2D;
            var c = new GUIContent(string.Concat("  ",title), iconTexture);
            return GUILayout.Button(c, Styles.ButtonIconStyle, options);
        }
        
        public static bool ButtonIconSmall(string title, LayoutIcon icon, params GUILayoutOption[] options)
        {
            return ButtonIcon(title, icon, GUILayout.ExpandWidth(false));
        }

        public static bool ButtonIconLarge(string title, LayoutIcon icon, params GUILayoutOption[] options)
        {

            return ButtonIcon(title, icon, GUILayout.ExpandWidth(true));
        }

        public static void SetShortLabelSize()
        {

            EditorGUIUtility.labelWidth = 100;
        }
        
        public static void SetDefaultLabelSize()
        {

            EditorGUIUtility.labelWidth = 150;
        }

        public static void SetExtendedLabelSize()
        {

            EditorGUIUtility.labelWidth = 300;
        }

        public static void BeginBackgroundColor(Color color)
        {
            backgroundColorTemp = GUI.backgroundColor;
            GUI.backgroundColor = color;
        }

        public static void EndBackgroundColor()
        {
            GUI.backgroundColor = backgroundColorTemp;
        }
    }
}