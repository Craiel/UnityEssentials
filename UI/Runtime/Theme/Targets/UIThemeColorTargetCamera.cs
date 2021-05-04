namespace Craiel.UnityEssentialsUI.Runtime.Theme.Targets
{
    using Enums;
    using UnityEngine;

    [RequireComponent(typeof(Camera))]
    public class UIThemeColorTargetCamera : UIThemeTarget
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SerializeField]
        public Camera Camera;

        [SerializeField]
        public ThemeColorType ColorType = ThemeColorType.FontDefault;

        [SerializeField]
        public bool KeepAlphaValue = true;
        
        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override bool Validate()
        {
            if (this.Camera == null)
            {
                this.Camera = this.GetComponent<Camera>();
                return this.Camera != null;
            }

            return false;
        }

        protected override bool DoApply(UITheme theme)
        {
            if (this.Camera == null)
            {
                return false;
            }

            Color value = theme.GetColorValue(this.ColorType);
            if (!this.KeepAlphaValue)
            {
                // Override the alpha value with the current value of the font
                value = new Color(value.r, value.g, value.b, this.Camera.backgroundColor.a);
            }
            
            if (this.Camera.backgroundColor == value)
            {
                return false;
            }

            this.Camera.backgroundColor = value;
            return true;
        }
    }
}