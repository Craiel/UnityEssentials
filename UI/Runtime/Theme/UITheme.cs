using UnityEngine;

namespace Craiel.UnityEssentialsUI.Runtime.Theme
{
    using TMPro;

    [CreateAssetMenu(menuName = "Craiel/UI/ColorScheme")]
    public class UITheme : ScriptableObject
    {
        [SerializeField]
        public string DisplayName;

        [Header("Fonts")]
        [SerializeField]
        public TMP_FontAsset PrimaryFont;
        
        [Header("Colors")]
        [SerializeField] 
        public Color PrimaryColor = Color.white;
        
        [SerializeField] 
        public Color SecondaryColor = Color.white;
        
        [SerializeField] 
        public Color Accent1Color = Color.white;
        
        [SerializeField] 
        public Color Accent2Color = Color.white;

        [SerializeField]
        public Color FontColorDefault = Color.white;
    }
}