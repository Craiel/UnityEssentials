namespace Craiel.UnityEssentials.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Craiel.UnityEssentials.Editor.UserInterface;
    using UnityEditor;
    using UnityEngine;

    public class BaseControllerPropertyEditor<T> : Editor
        where T : MonoBehaviour
    {
        private readonly IDictionary<string, Action> tabs;

        private string[] tabNames;

        private int selectedTabIndex;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public BaseControllerPropertyEditor()
        {
            this.tabs = new Dictionary<string, Action>();
            this.tabNames = new string[0];
            this.Title = "<ERR TITLE_NOT_SET>";
            this.ObjectTitle = this.Title;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Title { get; protected set; }

        public string ObjectTitle { get; protected set; }

        public void OnEnable()
        {
            this.Typed = this.target as T;
        }

        public override void OnInspectorGUI()
        {
            if (this.serializedObject.isEditingMultipleObjects == false)
            {
                this.serializedObject.Update();

                GUILayout.Label(this.Title, EditorStyles.boldLabel);
                GUILayout.BeginVertical(EditorStyles.helpBox);

                this.selectedTabIndex = GUILayout.SelectionGrid(this.selectedTabIndex, this.tabNames, this.tabNames.Length, EditorStyles.toolbarButton);

                this.tabs[this.tabNames[this.selectedTabIndex]].Invoke();

                GUILayout.EndVertical();
                this.serializedObject.ApplyModifiedProperties();
            }
            else
            {
                EditorGUILayout.HelpBox(string.Format("Can't edit multiple {0} at the same time", this.GetType().Name), MessageType.Warning);
            }

            GUILayout.Space(5);
            GUILayout.Label(this.ObjectTitle, EditorStyles.boldLabel);
            this.DrawDefaultInspector();
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected T Typed { get; private set; }

        protected void RegisterTab(string title, Action drawCallback)
        {
            this.tabs.Add(title, drawCallback);
            this.tabNames = this.tabs.Keys.ToArray();
        }

        protected Transform GetTransformByName(Transform[] candiates, params string[] matches)
        {
            foreach (var candidate in candiates)
            {
                foreach (var match in matches)
                {
                    if (candidate.name.ToLower().Contains(match.ToLower()))
                    {
                        return candidate;
                    }
                }
            }

            return null;
        }

        protected void DrawAutoFindComponent<TN>(Expression<Func<T, object>> expression) where TN : Component
        {
            EditorGUILayout.BeginHorizontal();
            this.DrawVisualDefProp(expression);
            if (GUILayout.Button("Find", GUILayout.Width(50)))
            {
                var comp = this.Typed.gameObject.GetComponentInChildren<TN>();
                if (comp != null)
                {
                    this.GetProp(expression).objectReferenceValue = comp;
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        protected void AutoFindComponents<TN>(SerializedProperty arrayProp) where TN : Component
        {
            foreach (var component in this.Typed.gameObject.GetComponentsInChildren<TN>(true))
            {
                var compAlreadyInArray = false;
                for (var i = 0; i < arrayProp.arraySize; i++)
                {
                    if (arrayProp.GetArrayElementAtIndex(i).objectReferenceValue == component)
                    {
                        compAlreadyInArray = true;
                        break;
                    }
                }

                if (compAlreadyInArray == false)
                {
                    arrayProp.arraySize++;
                    arrayProp.GetArrayElementAtIndex(arrayProp.arraySize - 1).objectReferenceValue = component;
                }
            }

            for (int i = arrayProp.arraySize - 1; i >= 0; i--)
            {
                if (arrayProp.GetArrayElementAtIndex(i).objectReferenceValue == null)
                {
                    arrayProp.DeleteArrayElementAtIndex(i);
                }
            }
        }

        protected SerializedProperty GetProp(Expression<Func<T, object>> expression)
        {
            return this.serializedObject.FindProperty(GuiUtils.GetFieldName(expression));
        }

        protected void DrawVisualDefProp(Expression<Func<T, object>> expression)
        {
            EditorGUILayout.PropertyField(this.GetProp(expression));
        }
    }
}
