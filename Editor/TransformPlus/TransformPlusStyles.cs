namespace Craiel.UnityEssentials.Editor.TransformPlus
{
    using UnityEditor;
    using UnityEngine;

    public static class TransformPlusStyles
    {
        private const int ButtonFontSize = 8;
        private const int ButtonFontSizeBold = 10;
        private const int ButtonHeight = 16;
        
        private static GUIStyle button;
        private static GUIStyle buttonBold;
        private static GUIStyle buttonLeft;
        private static GUIStyle buttonLeftBold;
        private static GUIStyle buttonRight;
        private static GUIStyle buttonRightBold;
        private static GUIStyle buttonMiddle;
        private static GUIStyle buttonMiddleBold;
        
        private static GUISkin skin;
        
        public static readonly Color ColorClear = new Color(1, 0.7f, 0.7f);
        public static readonly Color ColorSpace = new Color(0.7f, 1, 1);
        public static readonly Color ColorSnap = new Color(1, 0.7f, 1);
        public static readonly Color ColorCopy = new Color(0.7f, 1, 0.7f);
        public static readonly Color ColorPaste = new Color(1, 1, 0.7f);
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static GUIStyle Button
        {
            get
            {
                return button ??
                       (button = new GUIStyle(EditorStyles.miniButton)
                       {
                           fontSize = ButtonFontSize,
                           fontStyle = FontStyle.Bold,
                           fixedHeight = ButtonHeight
                       });
            }
        }
        
        public static GUIStyle ButtonBold
        {
            get
            {
                return buttonBold ??
                       (buttonBold = new GUIStyle(Button)
                       {
                           fontSize = ButtonFontSizeBold
                       });
            }
        }
        
        public static GUIStyle ButtonLeft
        {
            get
            {
                return buttonLeft ??
                       (buttonLeft = new GUIStyle(EditorStyles.miniButtonLeft)
                       {
                           fontSize = ButtonFontSize,
                           fontStyle = FontStyle.Bold,
                           fixedHeight = ButtonHeight
                       });
            }
        }
        
        public static GUIStyle ButtonLeftBold
        {
            get
            {
                return buttonLeftBold ??
                       (buttonLeftBold = new GUIStyle(ButtonLeft)
                       {
                           fontSize = ButtonFontSizeBold
                       });
            }
        }
        
        public static GUIStyle ButtonRight
        {
            get
            {
                return buttonRight ??
                       (buttonRight = new GUIStyle(EditorStyles.miniButtonRight)
                       {
                           fontSize = ButtonFontSize,
                           fontStyle = FontStyle.Bold,
                           fixedHeight = ButtonHeight
                       });
            }
        }
        
        public static GUIStyle ButtonRightBold
        {
            get
            {
                return buttonRightBold ??
                       (buttonRightBold = new GUIStyle(ButtonRight)
                       {
                           fontSize = ButtonFontSizeBold
                       });
            }
        }
        
        public static GUIStyle ButtonMiddle
        {
            get
            {
                return buttonMiddle ??
                       (buttonMiddle = new GUIStyle(EditorStyles.miniButtonMid)
                       {
                           fontSize = ButtonFontSize,
                           fontStyle = FontStyle.Bold,
                           fixedHeight = ButtonHeight
                       });
            }
        }

        public static GUIStyle ButtonMiddleBold
        {
            get
            {
                return buttonMiddleBold ??
                       (buttonMiddleBold = new GUIStyle(ButtonMiddle)
                       {
                           fontSize = ButtonFontSizeBold
                       });
            }
        }
        
        public static GUISkin Skin
        {
            get
            {
                if (skin == null)
                {
                    skin = Object.Instantiate(GUI.skin);
                    skin.button = Button;
                }

                return skin;
            }
        }
    }
}