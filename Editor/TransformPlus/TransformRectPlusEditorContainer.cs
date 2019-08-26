namespace TransformPlus
{
    using Craiel.UnityEssentials.Editor;
    using Craiel.UnityEssentials.Editor.TransformPlus;
    using Craiel.UnityEssentials.Runtime.Event;
    using Craiel.UnityEssentials.Runtime.Event.Editor;
    using UnityEditor;
    using UnityEngine.UIElements;
    using System;
    using System.Reflection;

    public class TransformRectPlusEditorContainer : EssentialEditorTemplateContainer
    {
        private Type baseEditorType;
        private Editor baseEditor;

        private BaseEventSubscriptionTicket selectionChangedTicket;

        private Button pasteButton;
        private Button pasteAnchorsButton;
        private Button pasteSizeButton;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public TransformRectPlusEditorContainer()
        {
            this.selectionChangedTicket = EditorEvents.Subscribe<TransformRectPlusSelectionChanged>(this.OnSelectionChanged);
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void Initialize()
        {
            base.Initialize();

            this.AddStyle("TransformPlus");

            Assembly ass = Assembly.GetAssembly(typeof(UnityEditor.Editor));
            this.baseEditorType = ass.GetType("UnityEditor.RectTransformEditor");
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void InitializeView(TemplateContainer view)
        {
            this.InitButton(view, "buttonReset", this.OnReset);

            var baseEditorContainer = new IMGUIContainer(this.OnBaseEditorGUI);
            view.Q<VisualElement>("baseEditorContainer").Add(baseEditorContainer);

            this.InitButton(view, "buttonCopy", this.OnCopy);
            this.InitButton(view, "buttonCopyAnchors", this.OnCopyAnchors);
            this.InitButton(view, "buttonCopySize", this.OnCopySize);

            this.pasteButton = this.InitButton(view, "buttonPaste", this.OnPaste);
            this.pasteAnchorsButton = this.InitButton(view, "buttonPasteAnchors", this.OnPasteAnchors);
            this.pasteSizeButton = this.InitButton(view, "buttonPasteSize", this.OnPasteSize);

            this.InitButton(view, "buttonResetAll", this.OnResetAll);
            this.InitButton(view, "buttonResetAnchors", this.OnResetAnchors);
            this.InitButton(view, "buttonResetSize", this.OnResetSize);
        }

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                EditorEvents.Unsubscribe(ref this.selectionChangedTicket);
            }

            base.Dispose(isDisposing);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void OnSelectionChanged(TransformRectPlusSelectionChanged eventData)
        {
            this.baseEditor = UnityEditor.Editor.CreateEditor(TransformRectPlus.Current, this.baseEditorType);
            this.Refresh();
        }

        private void OnBaseEditorGUI()
        {
            if (this.baseEditor == null)
            {
                return;
            }

            this.baseEditor.OnInspectorGUI();
        }

        private void OnReset()
        {
            Undo.RecordObject(TransformRectPlus.Current, "TransformRectPlus Reset");
            TransformRectPlus.Reset();
            this.Refresh();
        }

        private void OnCopySize()
        {
            TransformRectPlus.CopySize();
            this.Refresh();
        }

        private void OnCopyAnchors()
        {
            TransformRectPlus.CopyPivot();
            TransformRectPlus.CopyAnchorMin();
            TransformRectPlus.CopyAnchorMax();
            TransformRectPlus.CopyAnchorPosition();
            this.Refresh();
        }

        private void OnCopy()
        {
            TransformRectPlus.CopyPivot();
            TransformRectPlus.CopyAnchorMin();
            TransformRectPlus.CopyAnchorMax();
            TransformRectPlus.CopyAnchorPosition();
            TransformRectPlus.CopySize();
            this.Refresh();
        }

        private void OnResetSize()
        {
            Undo.RecordObject(TransformRectPlus.Current, "TransformRectPlus Reset Size");
            TransformRectPlus.ResetSize();
            this.Refresh();
        }

        private void OnResetAnchors()
        {
            Undo.RecordObject(TransformRectPlus.Current, "TransformRectPlus Reset Anchors");
            TransformRectPlus.ResetPivot();
            TransformRectPlus.ResetAnchorMin();
            TransformRectPlus.ResetAnchorMax();
            TransformRectPlus.ResetAnchorPosition();
            this.Refresh();
        }

        private void OnResetAll()
        {
            Undo.RecordObject(TransformRectPlus.Current, "TransformRectPlus Reset Transform");
            TransformRectPlus.ResetPivot();
            TransformRectPlus.ResetAnchorMin();
            TransformRectPlus.ResetAnchorMax();
            TransformRectPlus.ResetAnchorPosition();
            TransformRectPlus.ResetSize();
            this.Refresh();
        }

        private void OnPasteSize()
        {
            Undo.RecordObject(TransformRectPlus.Current, "TransformRectPlus Paste Size");
            TransformRectPlus.PasteSize();
            this.Refresh();
        }

        private void OnPasteAnchors()
        {
            Undo.RecordObject(TransformRectPlus.Current, "TransformRectPlus Paste Anchors");
            TransformRectPlus.PastePivot();
            TransformRectPlus.PasteAnchorMin();
            TransformRectPlus.PasteAnchorMax();
            TransformRectPlus.PasteAnchorPosition();
            this.Refresh();
        }

        private void OnPaste()
        {
            Undo.RecordObject(TransformRectPlus.Current, "TransformRectPlus Paste All");
            TransformRectPlus.PastePivot();
            TransformRectPlus.PasteAnchorMin();
            TransformRectPlus.PasteAnchorMax();
            TransformRectPlus.PasteAnchorPosition();
            TransformRectPlus.PasteSize();
            this.Refresh();
        }

        private void Refresh()
        {
            this.pasteButton.SetEnabled(TransformRectPlus.HasSizeCopy && TransformRectPlus.HasAnchorMinCopy && TransformRectPlus.HasAnchorMaxCopy && TransformRectPlus.HasAnchorPositionCopy);
            this.pasteAnchorsButton.SetEnabled(TransformRectPlus.HasAnchorMaxCopy);
            this.pasteSizeButton.SetEnabled(TransformRectPlus.HasSizeCopy);
        }
    }
}