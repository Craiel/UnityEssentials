using Craiel.UnityEssentials.Editor;

namespace Craiel.UnityEssentials.Runtime.Event.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;

    public class GameEventsWindow : EssentialEditorWindow<GameEventsWindow>
    {
        private const int EventLimit = 1000;
        private const int EventLimitByGroup = 250;

        private static readonly GUILayoutOption ColumnWidth = GUILayout.Width(50);

        private readonly IDictionary<Type, GameEventGroup> eventGroups;
        private readonly IList<GameEventInfo> events;

        private Texture2D errorIcon;
        private Texture2D warningIcon;
        private Texture2D infoIcon;

        private Vector2 scrollPosition;

        private bool displayGrouped;
        private bool displayReceivers;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public GameEventsWindow()
        {
            this.eventGroups = new Dictionary<Type, GameEventGroup>();
            this.events = new List<GameEventInfo>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [MenuItem("Window/Craiel/Game Events")]
        public static void ShowWindow()
        {
            OpenWindow();
        }

        public static void OpenWindow()
        {
            var window = (GameEventsWindow)GetWindow(typeof(GameEventsWindow));
            window.titleContent = new GUIContent("Game Events");
            window.Show();
        }

        public override void OnEnable()
        {
            base.OnEnable();

            this.eventGroups.Clear();

            GameEvents.DebugEventSend = this.OnEventSend;
        }

        public override void OnDestroy()
        {
            GameEvents.DebugEventSend = null;

            base.OnDestroy();
        }

        public void OnGUI()
        {
            this.errorIcon = EditorGUIUtility.FindTexture("d_console.erroricon.sml");
            this.warningIcon = EditorGUIUtility.FindTexture("d_console.warnicon.sml");
            this.infoIcon = EditorGUIUtility.FindTexture("d_console.infoicon.sml");

            this.DrawControls();
            this.DrawHeader();
            this.DrawResults();
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void OnEventSend(Type type, BaseEventSubscriptionTicket[] receivers)
        {
            GameEventGroup entry;
            if (!this.eventGroups.TryGetValue(type, out entry))
            {
                entry = new GameEventGroup(type);
                this.eventGroups.Add(type, entry);
            }

            GameEventInfo info = entry.Add(receivers);
            this.events.Add(info);

            while (this.events.Count > EventLimit)
            {
                this.events.RemoveAt(0);
            }
        }

        private void Clear()
        {
            this.eventGroups.Clear();
        }

        private void DrawControls()
        {
            GUILayout.Space(2);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear"))
            {
                this.Clear();
                return;
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            this.displayGrouped = GUILayout.Toggle(this.displayGrouped, "Grouped");
            this.displayReceivers = GUILayout.Toggle(this.displayReceivers, "Show Receivers");
            GUILayout.EndHorizontal();

            GUILayout.Space(2);
        }

        private void DrawHeader()
        {
            GUILayout.BeginHorizontal("box");
            GUILayout.Label("Type");
            GUILayout.FlexibleSpace();
            GUILayout.Label("Count", ColumnWidth);
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
        }

        private void DrawResults()
        {
            if (this.eventGroups.Count == 0)
            {
                GUILayout.Label("No Events to display");
                return;
            }

            this.scrollPosition = GUILayout.BeginScrollView(this.scrollPosition);

            if (this.displayGrouped)
            {
                foreach (GameEventGroup result in this.eventGroups.Values)
                {
                    this.DrawContentResult(result);
                }
            }
            else
            {
                foreach (GameEventInfo eventInfo in this.events)
                {
                    this.DrawContentEntry(eventInfo, true);
                }
            }

            GUILayout.EndScrollView();
        }

        private void DrawContentResult(GameEventGroup issueGroup)
        {
            GUILayout.BeginHorizontal("box");
            GUILayout.Space(10);
            GUILayout.Label(new GUIContent(this.infoIcon));
            GUILayout.Label(issueGroup.Type.ToString());
            GUILayout.FlexibleSpace();
            GUILayout.Label(issueGroup.Count.ToString(), ColumnWidth);
            GUILayout.EndHorizontal();

            var lastRect = GUILayoutUtility.GetLastRect();
            issueGroup.IsFoldout = GUI.Toggle(lastRect, issueGroup.IsFoldout, string.Empty);

            if (issueGroup.IsFoldout)
            {
                GUILayout.Space(10);
                for (var i = 0; i < issueGroup.Count; i++)
                {
                    this.DrawContentEntry(issueGroup.GetInfo(i));
                }

                GUILayout.Space(10);
            }
        }

        private void DrawContentEntry(GameEventInfo eventData, bool showParentInfo = false)
        {
            GUILayout.BeginHorizontal();

            if (showParentInfo)
            {
                GUILayout.Label(new GUIContent(this.infoIcon));

                GUILayout.Label(string.Format("[{0}] {1} ({2})", eventData.Time.ToShortTimeString(), eventData.Parent.Type, eventData.Receivers.Count.ToString()));

            }
            else
            {
                GUILayout.Space(20);

                GUILayout.Label(string.Format("[{0}] ({1})", eventData.Time.ToShortTimeString(), eventData.Receivers.Count.ToString()));
            }

            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();

            if (this.displayReceivers)
            {
                foreach (GameEventReceiverInfo receiver in eventData.Receivers)
                {
                    if (receiver == null)
                    {
                        continue;
                    }

                    GUILayout.BeginHorizontal();

                    GUILayout.Space(40);

                    GUILayout.Label(string.Format("{0}  -  {1}", receiver.TargetName, receiver.MethodName));

                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }

                if (eventData.Receivers.Count > 0)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(60);
                    GUILayout.Label("  ----  ");
                    GUILayout.EndHorizontal();
                }
            }
        }

        private class GameEventGroup
        {
            private const int ResizeIncrement = 10;

            private IList<GameEventInfo> events;

            public GameEventGroup(Type type)
            {
                this.Type = type;

                this.events = new List<GameEventInfo>();
            }

            public Type Type { get; private set; }

            public int Count { get; private set; }

            public int ReceiverCount { get; private set; }

            public bool IsFoldout { get; set; }

            public GameEventInfo GetInfo(int index)
            {
                return this.events[index];
            }

            public GameEventInfo Add(BaseEventSubscriptionTicket[] receivers)
            {

                var issue = new GameEventInfo(this, receivers);
                this.events.Add(issue);
                while (this.events.Count > EventLimitByGroup)
                {
                    this.events.RemoveAt(0);
                }

                this.Count++;
                this.ReceiverCount += issue.Receivers.Count;

                return issue;
            }
        }

        private class GameEventInfo
        {
            public GameEventInfo(GameEventGroup parent, BaseEventSubscriptionTicket[] receivers)
            {
                this.Parent = parent;

                this.Receivers = new List<GameEventReceiverInfo>();
                if (receivers != null)
                {
                    for (int i = 0; i < receivers.Length; i++)
                    {
                        if (receivers[i] == null)
                        {
                            continue;
                        }

                        this.Receivers.Add(new GameEventReceiverInfo(receivers[i]));
                    }
                }

                this.Time = DateTime.Now;
            }

            public DateTime Time { get; private set; }

            public GameEventGroup Parent { get; private set; }

            public IList<GameEventReceiverInfo> Receivers { get; private set; }
        }

        private class GameEventReceiverInfo
        {
            public GameEventReceiverInfo(BaseEventSubscriptionTicket receiver)
            {
                Type eventType = receiver.TargetDelegate.GetType();
                PropertyInfo methodProperty = eventType.GetProperty("Method");
                PropertyInfo targetProperty = eventType.GetProperty("Target");
                this.MethodName = methodProperty.GetValue(receiver.TargetDelegate, null).ToString();
                this.TargetName = targetProperty.GetValue(receiver.TargetDelegate, null).ToString();
            }

            public string MethodName { get; private set; }
            public string TargetName { get; private set; }
        }
    }
}