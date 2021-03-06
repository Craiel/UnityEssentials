﻿namespace Craiel.UnityEssentials.Editor
{
    using NLog;
    using Runtime.Logging;
    using UnityEditor;

    using UnityEngine;

    public partial class NLogConsole
    {
        private const string AllNameFilter = "All";
        
        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private bool FilterLog(NLogInterceptorEvent log)
        {
            if (!string.IsNullOrEmpty(this.nameFilter)
                && this.nameFilter != AllNameFilter
                && log.LoggerName != this.nameFilter)
            {
                return true;
            }
            
            if (this.filterRegex != null && !this.filterRegex.IsMatch(log.Message))
            {
                return true;
            }

            return false;
        }

        private bool FilterLogLevel(NLogInterceptorEvent log)
        {
            if ((log.Level == LogLevel.Info && this.showInfo)
                || (log.Level == LogLevel.Warn && this.showWarning)
                || (log.Level == LogLevel.Error && this.showError))
            {
                return false;
            }

            return true;
        }
        
        private void ClearSelectedMessage()
        {
            this.selectedLogIndex = -1;
            this.selectedCallstackFrame = -1;
            showFrameSource = false;
        }

        private void OnPlaymodeStateChanged(PlayModeStateChange stateChange)
        {
            if (!this.wasPlaying && EditorApplication.isPlayingOrWillChangePlaymode)
            {
                if (this.clearOnPlay)
                {
                    NLogInterceptor.Instance.Clear();
                }
            }

            this.wasPlaying = EditorApplication.isPlayingOrWillChangePlaymode;
        }

        private void ResizeTopPane()
        {
            // Set up the resize collision rect
            cursorChangeRect = new Rect(0, currentTopPaneHeight, position.width, dividerHeight);

            var oldColor = GUI.color;
            GUI.color = this.sizerLineColor;
            GUI.DrawTexture(cursorChangeRect, EditorGUIUtility.whiteTexture);
            GUI.color = oldColor;
            EditorGUIUtility.AddCursorRect(cursorChangeRect, MouseCursor.ResizeVertical);

            if (Event.current.type == EventType.MouseDown && cursorChangeRect.Contains(Event.current.mousePosition))
            {
                resize = true;
            }

            // If we've resized, store the new size and force a repaint
            if (resize)
            {
                currentTopPaneHeight = Event.current.mousePosition.y;
                cursorChangeRect.Set(cursorChangeRect.x, currentTopPaneHeight, cursorChangeRect.width, cursorChangeRect.height);
                Repaint();
            }

            if (Event.current.type == EventType.MouseUp)
            {
                resize = false;
            }

            currentTopPaneHeight = Mathf.Clamp(currentTopPaneHeight, 100, position.height - 100);
        }

        private class CountedLog
        {
            public CountedLog(NLogInterceptorEvent eventData, int count)
            {
                this.Event = eventData;
                this.Count = count;
            }

            public NLogInterceptorEvent Event { get; private set; }

            public int Count { get; set; }
        }
    }
}
