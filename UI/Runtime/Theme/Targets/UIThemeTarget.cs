namespace Craiel.UnityEssentialsUI.Runtime.Theme.Targets
{
    using UnityEngine;
    
#if UNITY_EDITOR
    using UnityEditor;
#endif

    public abstract class UIThemeTarget : MonoBehaviour
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public virtual void Awake() { }

        public virtual void OnEnable()
        {
            if (UIThemeSystem.IsInstanceActive)
            {
                UIThemeSystem.Instance.RegisterTarget(this);
            }
        }

        public virtual void OnDisable()
        {
            if (UIThemeSystem.IsInstanceActive)
            {
                UIThemeSystem.Instance.UnregisterTarget(this);
            }
        }
        
        public void OnValidate()
        {
            if (this.Validate())
            {
#if UNITY_EDITOR
                EditorUtility.SetDirty(this);
#endif
            }

            
            if (UIThemeSystem.IsInstanceActive && UIThemeSystem.Instance.ActiveTheme != null)
            {
                UIThemeSystem.Instance.ApplyThemeTo(this);
            }
        }


        public void Apply(UITheme theme)
        {
            if (!this.DoApply(theme))
            {
                return;
            }

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                EditorUtility.SetDirty(this);
            }
#endif
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected abstract bool Validate();
        
        protected abstract bool DoApply(UITheme theme);
    }
}