namespace Craiel.UnityEssentials.Editor
{
    using NLog;
    using NLog.Config;
    using Runtime.Logging;
    using UnityEditor;

    using UnityEngine;

    public partial class NLogConsole : EssentialEditorWindow<NLogConsole>
    {
        private readonly DrawingContext drawingContext = new DrawingContext();

        private Texture2D errorIcon;
        private Texture2D warningIcon;
        private Texture2D messageIcon;
        private Texture2D smallErrorIcon;
        private Texture2D smallWarningIcon;
        private Texture2D smallMessageIcon;

        private GUIStyle entryStyleBackEven;
        private GUIStyle entryStyleBackOdd;

        private Color sizerLineColor;

        private Vector2 logListScrollPosition;
        private Vector2 logDetailsScrollPosition;

        private string filterRegexText;
        private System.Text.RegularExpressions.Regex filterRegex;
        private bool showError = true;
        private bool showWarning = true;
        private bool showInfo = true;
        private bool hasChanged = true;
        private bool clearOnPlay = true;
        private bool wasPlaying;
        private bool collapse;
        private bool scrollFollowMessages = true;
        private bool limitMaxLines = true;
        private bool suspendDrawing;
        private bool pauseUpdate;
        private bool showFrameSource = true;
        private float currentTopPaneHeight = 200;
        private float dividerHeight = 5;
        private int selectedLogIndex = -1;
        private int selectedCallstackFrame = -1;
        private float logListMaxWidth;
        private float logListLineHeight;
        private float collapseBadgeMaxWidth;
        private bool resize;
        private Rect cursorChangeRect;

        private string nameFilter;

        private Vector2 drawPos;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [MenuItem("Window/Craiel/NLog Console")]
        public static void ShowWindow()
        {
            OpenWindow();
        }

        public static void OpenWindow()
        {
            var window = CreateInstance<NLogConsole>();
            window.titleContent = new GUIContent("NLog Console");
            window.Show();
            window.position = new Rect(200, 200, 400, 300);
            window.currentTopPaneHeight = window.position.height / 2;
        }

        public void OnInspectorUpdate()
        {
            if (this.hasChanged)
            {
                this.Repaint();
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();

            // Connect to or create the backend
            if (!NLogInterceptor.IsInstanceActive)
            {
                NLogInterceptor.InstantiateAndInitialize();
            }

            NLogInterceptor.Instance.OnLogChanged -= this.OnLogChanged;
            NLogInterceptor.Instance.OnLogChanged += this.OnLogChanged;

            if (LogManager.Configuration != null)
            {
                var config = new LoggingConfiguration();
                config.AddTarget("interceptor", NLogInterceptor.Instance.Target);

                var rule = new LoggingRule("*", LogLevel.Debug, NLogInterceptor.Instance.Target);
                config.LoggingRules.Add(rule);

                LogManager.Configuration = config;
            }

            this.titleContent.text = "NLog Console";

            EditorApplication.playModeStateChanged += this.OnPlaymodeStateChanged;

            this.ClearSelectedMessage();

            this.smallErrorIcon = EditorGUIUtility.FindTexture("d_console.erroricon.sml");
            this.smallWarningIcon = EditorGUIUtility.FindTexture("d_console.warnicon.sml");
            this.smallMessageIcon = EditorGUIUtility.FindTexture("d_console.infoicon.sml");

            this.errorIcon = this.smallErrorIcon;
            this.warningIcon = this.smallWarningIcon;
            this.messageIcon = this.smallMessageIcon;
            this.hasChanged = true;
            this.Repaint();
        }

        private void OnLogChanged(NLogInterceptorEvent eventData)
        {
            this.hasChanged = true;
        }

        public void OnGUI()
        {
            // Set up the basic style, based on the Unity defaults
            // A bit hacky, but means we don't have to ship an editor guistyle and can fit in to pro and free skins
            Color defaultLineColor = GUI.backgroundColor;
            GUIStyle unityLogLineEven = null;
            GUIStyle unityLogLineOdd = null;
            GUIStyle unitySmallLogLine = null;

            foreach (var style in GUI.skin.customStyles)
            {
                if (style.name == "CN EntryBackEven")
                {
                    unityLogLineEven = style;
                }
                else if (style.name == "CN EntryBackOdd")
                {
                    unityLogLineOdd = style;
                }
                else if (style.name == "CN StatusInfo")
                {
                    unitySmallLogLine = style;
                }
            }

            this.entryStyleBackEven = new GUIStyle(unitySmallLogLine)
                                          {
                                              normal = unityLogLineEven.normal,
                                              margin = new RectOffset(0, 0, 0, 0),
                                              border = new RectOffset(0, 0, 0, 0),
                                              fixedHeight = 0
                                          };

            this.entryStyleBackOdd = new GUIStyle(this.entryStyleBackEven) { normal = unityLogLineOdd.normal };


            this.sizerLineColor = new Color(defaultLineColor.r * 0.5f, defaultLineColor.g * 0.5f, defaultLineColor.b * 0.5f);

            // GUILayout.BeginVertical(GUILayout.Height(topPanelHeaderHeight), GUILayout.MinHeight(topPanelHeaderHeight));
            this.ResizeTopPane();
            this.drawPos = Vector2.zero;
            this.DrawToolbar();
            this.DrawFilter();

            this.DrawNames();

            float logPanelHeight = this.currentTopPaneHeight - this.drawPos.y;

            if (!this.suspendDrawing)
            {
                this.DrawLogList(logPanelHeight);

                this.drawPos.y += this.dividerHeight;

                this.DrawLogDetails();
            }

            // If we're dirty, do a repaint
            this.hasChanged = true;
            this.Repaint();
        }
    }
}
