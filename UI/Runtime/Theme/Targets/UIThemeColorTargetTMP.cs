namespace Craiel.UnityEssentialsUI.Runtime.Theme.Targets
{
    using Enums;
    using TMPro;
    using UnityEngine;

    [RequireComponent(typeof(TMP_Text))]
    public class UIThemeColorTargetTMP : UIThemeTarget
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SerializeField]
        public TMP_Text Text;

        [SerializeField]
        public ThemeColorType ColorType = ThemeColorType.FontDefault;

        [SerializeField]
        public bool KeepAlphaValue = true;
        
        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override bool Validate()
        {
            if (this.Text == null)
            {
                this.Text = this.GetComponent<TMP_Text>();
                return this.Text != null;
            }

            return false;
        }

        protected override bool DoApply(UITheme theme)
        {
            if (Text == null)
            {
                return false;
            }

            Color value = theme.GetColorValue(this.ColorType);
            if (!this.KeepAlphaValue)
            {
                // Override the alpha value with the current value of the font
                value = new Color(value.r, value.g, value.b, this.Text.color.a);
            }
            
            if (this.Text.color == value)
            {
                return false;
            }

            this.Text.color = value;
            return true;
        }
    }
}