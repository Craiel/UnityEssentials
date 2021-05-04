namespace Craiel.UnityEssentialsUI.Runtime.Theme
{
    using Targets;
    using UnityEditor;
    using UnityEngine;
    using UnityEssentials.Runtime.Singletons;

    public class UIThemeSystem : UnitySingletonBehavior<UIThemeSystem>
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SerializeField]
        public UITheme ActiveTheme;

        public void SetTheme(UITheme newScheme)
        {
            this.ActiveTheme = newScheme;
                
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }

        public void ApplyTheme()
        {
            UIThemeTarget[] elements = FindObjectsOfType<UIThemeTarget>();
            foreach (UIThemeTarget element in elements)
            {
                ApplyThemeTo(element);
            }
        }

        public void ApplyThemeTo(UIThemeTarget target)
        {
            target.Apply(this.ActiveTheme);
        }

        public void RegisterTarget(UIThemeTarget target)
        {
        }

        public void UnregisterTarget(UIThemeTarget target)
        {
        }
    }
}