using System;
using System.Collections.Generic;
using Craiel.UnityEssentials.Runtime.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Craiel.UnityEssentials.Editor
{
    using Runtime;

    public abstract class EssentialEditorTemplateContainer : TemplateContainer, IDisposable
    {
        private const string ViewExtension = ".uxml";
        private const string StyleExtension = ".uss";

        private readonly Type editorType;

        private readonly ManagedDirectory viewPath;
        private readonly ManagedDirectory stylePath;
        private readonly ManagedDirectory assetPath;

        private readonly IDictionary<string, StyleSheet> additionalStyles;

        private VisualTreeAsset view;
        private StyleSheet styleSheet;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        protected EssentialEditorTemplateContainer(Type customType)
            : this()
        {
            this.editorType = customType;
        }

        protected EssentialEditorTemplateContainer()
        {
            this.editorType = this.GetType();

            this.viewPath = EssentialsEditorConstants.UIPath.ToDirectory("Views");
            this.stylePath = EssentialsEditorConstants.UIPath.ToDirectory("Styles");
            this.assetPath = EssentialsEditorConstants.UIPath.ToDirectory("Assets");

            this.additionalStyles = new Dictionary<string, StyleSheet>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public virtual void Initialize()
        {
            if (!this.LoadView(this.editorType.Name, ref this.view))
            {
                UnityEngine.Debug.LogError("Container Initialize failed, no view asset loaded!");
                return;
            }

            if (this.LoadStyle(this.editorType.Name, ref styleSheet))
            {
                this.styleSheets.Add(this.styleSheet);
            }

            this.style.flexGrow = 1;

            TemplateContainer container = this.view.CloneTree();
            this.InitializeView(container);
            this.Add(container);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void AddCustomContent(VisualElement element)
        {
            this.Add(element);
        }

        public void AddCustomIMGUIContent(Action onGuiHandler)
        {
            var container = new IMGUIContainer(onGuiHandler);
            this.Add(container);
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected void AddStyle(string styleName)
        {
            if (this.additionalStyles.ContainsKey(styleName))
            {
                UnityEngine.Debug.LogWarningFormat("Style {0} is already loaded", styleName);
                return;
            }

            StyleSheet newStyle = null;
            if (this.LoadStyle(styleName, ref newStyle, false))
            {
                this.additionalStyles.Add(styleName, newStyle);
                this.styleSheets.Add(newStyle);
            }
        }

        protected Type EditorType => this.editorType;

        protected abstract void InitializeView(TemplateContainer view);

        protected bool LoadView<T>(ref VisualTreeAsset target)
        {
            return this.LoadView(TypeCache<T>.Value.Name, ref target);
        }

        protected bool LoadView(string viewName, ref VisualTreeAsset target)
        {
            ManagedFile viewFile = this.viewPath.ToFile(viewName + ViewExtension);
            return this.DoLoadAsset(viewFile, ref target);
        }

        protected bool LoadStyle<T>(ref StyleSheet target, bool silent = true)
        {
            return this.LoadStyle(TypeCache<T>.Value.Name, ref target, silent);
        }

        protected bool LoadStyle(string styleName, ref StyleSheet target, bool silent = true)
        {
            ManagedFile styleFile = this.stylePath.ToFile(styleName + StyleExtension);
            return this.DoLoadAsset(styleFile, ref target, silent);
        }

        protected bool LoadAsset<T>(string relativePath, ref T target)
            where T : UnityEngine.Object
        {
            ManagedFile assetFile = this.assetPath.ToFile(relativePath);
            return this.DoLoadAsset(assetFile, ref target);
        }

        protected Button InitButton(TemplateContainer target, string name, System.Action callback)
        {
            var control = target.Q<Button>(name);
            control.RegisterCallback<MouseUpEvent>(x =>
            {
                if (x.button != (int) MouseButton.LeftMouse
                    || x.modifiers != EventModifiers.None)
                {
                    return;
                }

                callback();
            });

            return control;
        }

        protected Toggle InitToggle(TemplateContainer target, string name, System.Action<bool> callback)
        {
            var control = target.Q<Toggle>(name);
            control.RegisterCallback<ChangeEvent<bool>>(x => { callback(x.newValue); });
            return control;
        }

        protected Slider InitSlider(TemplateContainer target, string name, System.Action<float> callback)
        {
            var control = target.Q<Slider>(name);
            control.RegisterCallback<ChangeEvent<float>>(x => { callback(x.newValue); });
            return control;
        }

        protected Vector3Field InitVector3(TemplateContainer target, string name, System.Action<Vector3> callback)
        {
            var control = target.Q<Vector3Field>(name);
            control.RegisterCallback<ChangeEvent<Vector3>>(x => { callback(x.newValue); });
            return control;
        }

        protected virtual void Dispose(bool isDisposing)
        {
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private bool DoLoadAsset<T>(ManagedFile file, ref T target, bool silent = false)
            where T : UnityEngine.Object
        {
            if (target != null)
            {
                // Asset is already loaded
                return true;
            }

            target = AssetDatabase.LoadAssetAtPath<T>(file.GetPath());
            if (target == null)
            {
                if (!silent)
                {
                    UnityEngine.Debug.LogErrorFormat("Failed to Load Asset: {0}", file);
                }

                return false;
            }

            return true;
        }
    }
}