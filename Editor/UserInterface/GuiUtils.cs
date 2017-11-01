namespace Assets.Scripts.Craiel.Essentials.Editor.UserInterface
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;

    public static class GuiUtils
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------

        // Some helper functions to draw buttons that are only as big as their text
        public static bool ButtonClamped(Vector2 drawPosition, string text, GUIStyle style, out Vector2 size)
        {
            var content = new GUIContent(text);
            size = style.CalcSize(content);
            var rect = new Rect(drawPosition, size);
            return GUI.Button(rect, text, style);
        }

        public static bool ToggleClamped(Vector2 drawPosition, bool state, string text, GUIStyle style, out Vector2 size)
        {
            var content = new GUIContent(text);
            return ToggleClamped(drawPosition, state, content, style, out size);
        }

        public static bool ToggleClamped(Vector2 drawPosition, bool state, GUIContent content, GUIStyle style, out Vector2 size)
        {
            size = style.CalcSize(content);
            Rect drawRect = new Rect(drawPosition, size);
            return GUI.Toggle(drawRect, state, content, style);
        }

        public static void LabelClamped(Vector2 drawPosition, string text, GUIStyle style, out Vector2 size)
        {
            var content = new GUIContent(text);
            size = style.CalcSize(content);

            Rect drawRect = new Rect(drawPosition, size);
            GUI.Label(drawRect, text, style);
        }

        public static string GetPropertyName<TSource>(Expression<Func<TSource, object>> expression)
        {
            var lambda = expression as LambdaExpression;
            MemberExpression memberExpression;
            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = lambda.Body as UnaryExpression;
                memberExpression = unaryExpression.Operand as MemberExpression;
            }
            else
            {
                memberExpression = lambda.Body as MemberExpression;
            }

            Debug.Assert(memberExpression != null, "Please provide a lambda expression like 'n => n.PropertyName'");

            if (memberExpression != null)
            {
                var propertyInfo = memberExpression.Member as PropertyInfo;

                return propertyInfo.Name;
            }

            return null;
        }

        public static string GetFieldName<TSource>(Expression<Func<TSource, object>> expression)
        {
            var lambda = expression as LambdaExpression;
            MemberExpression memberExpression;
            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = lambda.Body as UnaryExpression;
                memberExpression = unaryExpression.Operand as MemberExpression;
            }
            else
            {
                memberExpression = lambda.Body as MemberExpression;
            }

            Debug.Assert(memberExpression != null, "Please provide a lambda expression like 'n => n.PropertyName'");

            if (memberExpression != null)
            {
                var fieldInfo = memberExpression.Member as FieldInfo;
                return fieldInfo.Name;
            }

            return null;
        }

        public static SerializedProperty FindProperty<TSource>(
            this SerializedObject source,
            Expression<Func<TSource, object>> expression)
        {
            return source.FindProperty(GetFieldName(expression));
        }

        public static SerializedProperty FindPropertyRelative<TSource>(
            this SerializedProperty source,
            Expression<Func<TSource, object>> expression)
        {
            return source.FindPropertyRelative(GetFieldName(expression));
        }
    }
}
