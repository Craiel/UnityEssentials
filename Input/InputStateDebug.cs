namespace Assets.Scripts.Craiel.Essentials.Input
{
    using UnityEngine;

    public class InputStateDebug : BaseInputState
    {
        public static readonly InputControl DebugConfirm = new InputControl("DBG_Confirm");
        public static readonly InputControl DebugCancel = new InputControl("DBG_Cancel");
        public static readonly InputControl DebugToggleMenu = new InputControl("DBG_ToggleMenu");

        public static readonly InputControl DebugSelect1 = new InputControl("DBG_Select1");
        public static readonly InputControl DebugSelect2 = new InputControl("DBG_Select2");
        public static readonly InputControl DebugSelect3 = new InputControl("DBG_Select3");
        public static readonly InputControl DebugSelect4 = new InputControl("DBG_Select4");
        public static readonly InputControl DebugSelect5 = new InputControl("DBG_Select5");

        // Needs to come after the Controls
        public static readonly InputStateDebug Instance = new InputStateDebug();
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Initialize()
        {
            // Register the controls
            InputHandler.Instance.RegisterControl(DebugConfirm);
            InputHandler.Instance.RegisterControl(DebugCancel);
            InputHandler.Instance.RegisterControl(DebugToggleMenu);

            InputHandler.Instance.RegisterControl(DebugSelect1);
            InputHandler.Instance.RegisterControl(DebugSelect2);
            InputHandler.Instance.RegisterControl(DebugSelect3);
            InputHandler.Instance.RegisterControl(DebugSelect4);
            InputHandler.Instance.RegisterControl(DebugSelect5);

            // Now map they keys
            this.AddKey(KeyCode.Mouse0).For(DebugConfirm);
            this.AddKey(KeyCode.Mouse1).For(DebugCancel);
            this.AddKey(KeyCode.Escape).For(DebugToggleMenu);

            this.AddKey(KeyCode.Alpha1).For(DebugSelect1);
            this.AddKey(KeyCode.Alpha2).For(DebugSelect2);
            this.AddKey(KeyCode.Alpha3).For(DebugSelect3);
            this.AddKey(KeyCode.Alpha4).For(DebugSelect4);
            this.AddKey(KeyCode.Alpha5).For(DebugSelect5);
        }
    }
}
