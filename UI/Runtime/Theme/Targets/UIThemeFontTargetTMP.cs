namespace Craiel.UnityEssentialsUI.Runtime.Theme.Targets
{
    using TMPro;
    using UnityEngine;

    [RequireComponent(typeof(TMP_Text))]
    public class UIThemeFontTargetTMP : UIThemeTarget
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SerializeField]
        public TMP_Text Text;
        
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

            if (this.Text.font == theme.PrimaryFont)
            {
                return false;
            }
            
            this.Text.font = theme.PrimaryFont;
            return true;
        }
    }
}