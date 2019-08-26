namespace Craiel.UnityEssentials.Editor.TransformPlus
{
    using Craiel.UnityEssentials.Runtime.Event;
    using Craiel.UnityEssentials.Runtime.Event.Editor;
    using UnityEditor;
    using UnityEditor.UIElements;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class TransformPlusEditorContainer : EssentialEditorTemplateContainer
    {
        private BaseEventSubscriptionTicket selectionChangedTicket;

        private Button modeLocalButton;
        private Button modeWorldButton;

        private Vector3Field positionField;
        private Vector3Field rotationField;
        private Vector3Field scaleField;

        private Button pasteButton;
        private Button pastePositionButton;
        private Button pasteRotationButton;
        private Button pasteScaleButton;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public TransformPlusEditorContainer()
        {
            this.selectionChangedTicket = EditorEvents.Subscribe<TransformPlusSelectionChanged>(this.OnSelectionChanged);
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void Initialize()
        {
            base.Initialize();

            this.AddStyle("TransformPlus");
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void InitializeView(TemplateContainer view)
        {
            this.modeLocalButton = this.InitButton(view, "buttonModeLocal", () => this.OnSpaceChanged(TransformPlusSpace.Local));
            this.modeWorldButton = this.InitButton(view, "buttonModeWorld", () => this.OnSpaceChanged(TransformPlusSpace.World));

            this.InitButton(view, "buttonTogglePivot",() => TransformPlus.DrawPivot = !TransformPlus.DrawPivot);
            this.InitButton(view, "buttonReset", this.OnReset);

            this.positionField = this.InitVector3(view, "position", this.OnPositionChanged);
            this.InitButton(view, "buttonResetPosition", () => this.OnPositionChanged(Vector3.zero));

            this.rotationField = this.InitVector3(view, "rotation", this.OnRotationChanged);
            this.InitButton(view, "buttonResetRotation", () => this.OnRotationChanged(Vector3.zero));

            this.scaleField = this.InitVector3(view, "scale", this.OnScaleChanged);
            this.InitButton(view, "buttonResetScale", () => this.OnScaleChanged(Vector3.one));

            this.InitButton(view, "buttonSnap", this.OnSnap);
            this.InitButton(view, "buttonSnapPosition", this.OnSnapPosition);
            this.InitButton(view, "buttonSnapRotation", this.OnSnapRotation);

            this.InitButton(view, "buttonCopy", this.OnCopy);
            this.InitButton(view, "buttonCopyPosition", this.OnCopyPosition);
            this.InitButton(view, "buttonCopyRotation", this.OnCopyRotation);
            this.InitButton(view, "buttonCopyScale", this.OnCopyScale);

            this.pasteButton = this.InitButton(view, "buttonPaste", this.OnPaste);
            this.pastePositionButton = this.InitButton(view, "buttonPastePosition", this.OnPastePosition);
            this.pasteRotationButton = this.InitButton(view, "buttonPasteRotation", this.OnPasteRotation);
            this.pasteScaleButton = this.InitButton(view, "buttonPasteScale", this.OnPasteScale);
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
        private void OnSelectionChanged(TransformPlusSelectionChanged eventData)
        {
            this.Refresh();
        }

        private void OnScaleChanged(Vector3 value)
        {
            Undo.RecordObject(TransformPlus.Current, "TransformPlus Move");
            TransformPlus.SetScale(value);
            this.Refresh();
        }

        private void OnRotationChanged(Vector3 value)
        {
            Undo.RecordObject(TransformPlus.Current, "TransformPlus Move");
            TransformPlus.SetRotation(value);
            this.Refresh();
        }

        private void OnPositionChanged(Vector3 value)
        {
            Undo.RecordObject(TransformPlus.Current, "TransformPlus Move");
            TransformPlus.SetPosition(value);
            this.Refresh();
        }

        private void OnSpaceChanged(TransformPlusSpace value)
        {
            TransformPlus.Space = value;
            this.Refresh();
        }

        private void OnReset()
        {
            Undo.RecordObject(TransformPlus.Current, "TransformPlus Reset");
            TransformPlus.Reset();
            this.Refresh();
        }

        private void OnSnap()
        {
            Undo.RecordObject(TransformPlus.Current, "TransformPlus Snap");
            TransformPlus.SetPositionSnapped(TransformPlus.Position);
            TransformPlus.SetRotationSnapped(TransformPlus.Rotation);
            this.Refresh();
        }

        private void OnSnapRotation()
        {
            Undo.RecordObject(TransformPlus.Current, "TransformPlus Snap Rotation");
            TransformPlus.SetRotationSnapped(TransformPlus.Rotation);
            this.Refresh();
        }

        private void OnSnapPosition()
        {
            Undo.RecordObject(TransformPlus.Current, "TransformPlus Snap Position");
            TransformPlus.SetPositionSnapped(TransformPlus.Position);
            this.Refresh();
        }

        private void OnCopyScale()
        {
            TransformPlus.CopyScale();
            this.Refresh();
        }

        private void OnCopyRotation()
        {
            TransformPlus.CopyRotation();
            this.Refresh();
        }

        private void OnCopyPosition()
        {
            TransformPlus.CopyPosition();
            this.Refresh();
        }

        private void OnCopy()
        {
            TransformPlus.CopyPosition();
            TransformPlus.CopyRotation();
            TransformPlus.CopyScale();
            this.Refresh();
        }

        private void OnPasteScale()
        {
            Undo.RecordObject(TransformPlus.Current, "TransformPlus Paste Scale");
            TransformPlus.PasteScale();
            this.Refresh();
        }

        private void OnPasteRotation()
        {
            Undo.RecordObject(TransformPlus.Current, "TransformPlus Paste Rotate");
            TransformPlus.PasteRotation();
            this.Refresh();
        }

        private void OnPastePosition()
        {
            Undo.RecordObject(TransformPlus.Current, "TransformPlus Paste Position");
            TransformPlus.PastePosition();
            this.Refresh();
        }

        private void OnPaste()
        {
            Undo.RecordObject(TransformPlus.Current, "TransformPlus Paste Transform");
            TransformPlus.PastePosition();
            TransformPlus.PasteRotation();
            TransformPlus.PasteScale();
            this.Refresh();
        }

        private void Refresh()
        {
            if (TransformPlus.Space == TransformPlusSpace.Local)
            {
                this.modeLocalButton.SetEnabled(false);
                this.modeWorldButton.SetEnabled(true);
            }
            else
            {
                this.modeLocalButton.SetEnabled(true);
                this.modeWorldButton.SetEnabled(false);
            }

            this.positionField.value = TransformPlus.Position;
            this.rotationField.value = TransformPlus.Rotation;
            this.scaleField.value = TransformPlus.Scale;

            this.pasteButton.SetEnabled(TransformPlus.HasScaleCopy && TransformPlus.HasPositionCopy && TransformPlus.HasRotationCopy);
            this.pastePositionButton.SetEnabled(TransformPlus.HasPositionCopy);
            this.pasteRotationButton.SetEnabled(TransformPlus.HasRotationCopy);
            this.pasteScaleButton.SetEnabled(TransformPlus.HasScaleCopy);
        }
    }
}