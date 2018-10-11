namespace Craiel.UnityEssentials.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;

    using NLog;
    using Runtime.Extensions;
    using Runtime.Logging;
    using UnityEditor;

    using UnityEngine;

    using UserInterface;

    public partial class NLogConsole
    {
        private const int RenderLineLimit = 250;
        
        private readonly IList<NLogConsole.CountedLog> renderList = new List<NLogConsole.CountedLog>();
        
        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void DrawToolbar()
        {
            var toolbarStyle = EditorStyles.toolbarButton;

            Vector2 elementSize;
            if (GuiUtils.ButtonClamped(this.drawPos, "Clear", EditorStyles.toolbarButton, out elementSize))
            {
                this.forceRepaint = true;
                NLogInterceptor.Instance.Clear();
            }

            drawPos.x += elementSize.x;
            this.clearOnPlay = GuiUtils.ToggleClamped(this.drawPos, this.clearOnPlay, "Clear On Play", EditorStyles.toolbarButton, out elementSize);
            drawPos.x += elementSize.x;
            NLogInterceptor.Instance.PauseOnError = GuiUtils.ToggleClamped(this.drawPos, NLogInterceptor.Instance.PauseOnError, "Error Pause", EditorStyles.toolbarButton, out elementSize);
            drawPos.x += elementSize.x;
            
            var newCollapse = GuiUtils.ToggleClamped(this.drawPos, this.collapse, "Collapse", EditorStyles.toolbarButton, out elementSize);
            if (newCollapse != this.collapse)
            {
                this.forceRepaint = true;
                this.collapse = newCollapse;
                selectedLogIndex = -1;
            }

            drawPos.x += elementSize.x;

            this.scrollFollowMessages = GuiUtils.ToggleClamped(this.drawPos, this.scrollFollowMessages, "Follow", EditorStyles.toolbarButton, out elementSize);
            drawPos.x += elementSize.x;

            this.limitMaxLines = GuiUtils.ToggleClamped(this.drawPos, this.limitMaxLines, "Limit Lines", EditorStyles.toolbarButton, out elementSize);
            drawPos.x += elementSize.x;
            
            this.suspendDrawing = GuiUtils.ToggleClamped(this.drawPos, this.suspendDrawing, "Suspend Drawing", EditorStyles.toolbarButton, out elementSize);
            drawPos.x += elementSize.x;

            this.pauseUpdate = GuiUtils.ToggleClamped(this.drawPos, this.pauseUpdate, "Pause Update", EditorStyles.toolbarButton, out elementSize);
            drawPos.x += elementSize.x;

            var errorToggleContent = new GUIContent(NLogInterceptor.Instance.GetCount(LogLevel.Error).ToString(), smallErrorIcon);
            var warningToggleContent = new GUIContent(NLogInterceptor.Instance.GetCount(LogLevel.Warn).ToString(), smallWarningIcon);
            var messageToggleContent = new GUIContent(NLogInterceptor.Instance.GetCount(LogLevel.Info).ToString(), smallMessageIcon);

            float totalErrorButtonWidth = toolbarStyle.CalcSize(errorToggleContent).x + toolbarStyle.CalcSize(warningToggleContent).x + toolbarStyle.CalcSize(messageToggleContent).x;

            float errorIconX = position.width - totalErrorButtonWidth;
            if (errorIconX > drawPos.x)
            {
                drawPos.x = errorIconX;
            }

            var showErrors = GuiUtils.ToggleClamped(this.drawPos, this.showError, errorToggleContent, toolbarStyle, out elementSize);
            drawPos.x += elementSize.x;
            var showWarnings = GuiUtils.ToggleClamped(this.drawPos, this.showWarning, warningToggleContent, toolbarStyle, out elementSize);
            drawPos.x += elementSize.x;
            var showMessages = GuiUtils.ToggleClamped(this.drawPos, this.showInfo, messageToggleContent, toolbarStyle, out elementSize);
            drawPos.x += elementSize.x;

            drawPos.y += elementSize.y;
            drawPos.x = 0;

            // If the errors/warning to show has changed, clear the selected message
            if (showErrors != this.showError || showWarnings != this.showWarning || showMessages != this.showInfo)
            {
                ClearSelectedMessage();
                this.forceRepaint = true;
            }

            this.showWarning = showWarnings;
            this.showInfo = showMessages;
            this.showError = showErrors;
        }
        
        private void DrawNames()
        {
            var names = new List<string> { AllNameFilter };
            names.AddRange(NLogInterceptor.Instance.Names);

            int currentNameIndex = 0;
            for (int i = 0; i < names.Count; i++)
            {
                if (names[i] == this.nameFilter)
                {
                    currentNameIndex = i;
                    break;
                }
            }

            var content = new GUIContent("S");
            var size = GUI.skin.button.CalcSize(content);
            var drawRect = new Rect(drawPos, new Vector2(position.width, size.y));
            currentNameIndex = GUI.SelectionGrid(drawRect, currentNameIndex, names.ToArray(), names.Count);
            if (this.nameFilter != names[currentNameIndex])
            {
                this.nameFilter = names[currentNameIndex];
                ClearSelectedMessage();
                this.forceRepaint = true;
            }

            drawPos.y += size.y;
        }

        private void UpdateRenderList()
        {
            var logLineStyle = entryStyleBackEven;
            logListMaxWidth = 0;
            logListLineHeight = 0;
            collapseBadgeMaxWidth = 0;
            this.renderList.Clear();

            // When collapsed, count up the unique elements and use those to display
            if (collapse)
            {
                var collapsedLines = new Dictionary<string, CountedLog>();
                var collapsedLinesList = new List<CountedLog>();

                for (var i = NLogInterceptor.Instance.Events.Count - 1; i > 0; i--)
                {
                    NLogInterceptorEvent logEvent = NLogInterceptor.Instance.Events[i];
                    if (!FilterLogLevel(logEvent) && !FilterLog(logEvent))
                    {
                        var matchString = string.Concat(logEvent.Message, "!$", logEvent.Level, "!$", logEvent.LoggerName);

                        CountedLog countedLog;
                        if (collapsedLines.TryGetValue(matchString, out countedLog))
                        {
                            countedLog.Count++;
                        }
                        else
                        {
                            countedLog = new CountedLog(logEvent, 1);
                            collapsedLines.Add(matchString, countedLog);
                            collapsedLinesList.Add(countedLog);
                            if (this.limitMaxLines && collapsedLinesList.Count > RenderLineLimit)
                            {
                                break;
                            }
                        }
                    }
                }

                foreach (var countedLog in collapsedLinesList)
                {
                    var content = this.GetLogLineGUIContent(countedLog.Event);
                    this.renderList.Add(countedLog);
                    var logLineSize = logLineStyle.CalcSize(content);
                    logListMaxWidth = Mathf.Max(logListMaxWidth, logLineSize.x);
                    logListLineHeight = Mathf.Max(logListLineHeight, logLineSize.y);

                    var collapseBadgeContent = new GUIContent(countedLog.Count.ToString());
                    var collapseBadgeSize = EditorStyles.miniButton.CalcSize(collapseBadgeContent);
                    collapseBadgeMaxWidth = Mathf.Max(collapseBadgeMaxWidth, collapseBadgeSize.x);
                }
            }
            else
            {
                // If we're not collapsed, display everything in order
                for (var i = NLogInterceptor.Instance.Events.Count - 1; i > 0; i--)
                {
                    NLogInterceptorEvent logEvent = NLogInterceptor.Instance.Events[i];
                    if (!FilterLogLevel(logEvent) && !FilterLog(logEvent))
                    {
                        var content = this.GetLogLineGUIContent(logEvent);
                        this.renderList.Add(new CountedLog(logEvent, 1));
                        var logLineSize = logLineStyle.CalcSize(content);
                        logListMaxWidth = Mathf.Max(logListMaxWidth, logLineSize.x);
                        logListLineHeight = Mathf.Max(logListLineHeight, logLineSize.y);

                        if (this.limitMaxLines && this.renderList.Count > RenderLineLimit)
                        {
                            break;
                        }
                    }
                }
            }

            logListMaxWidth += collapseBadgeMaxWidth;

            // Have to reverse the render list if we limit the max line count
            IList<CountedLog> reversed = this.renderList.Reverse().ToList();
            this.renderList.Clear();
            this.renderList.AddRange(reversed);
        }
        
        private void DrawLogList(float height)
        {
            var oldColor = GUI.backgroundColor;

            var collapseBadgeStyle = EditorStyles.miniButton;
            
            // If we've been marked dirty, we need to recalculate the elements to be displayed
            if (this.hasChanged && !this.pauseUpdate)
            {
                this.UpdateRenderList();
            }
            
            var scrollRect = new Rect(drawPos, new Vector2(position.width, height));
            float lineWidth = Mathf.Max(logListMaxWidth, scrollRect.width);

            var contentRect = new Rect(0, 0, lineWidth, this.renderList.Count * logListLineHeight);
            Vector2 lastScrollPosition = logListScrollPosition;
            logListScrollPosition = GUI.BeginScrollView(scrollRect, logListScrollPosition, contentRect);

            // If we're following the messages but the user has moved, cancel following
            if (scrollFollowMessages)
            {
                if (lastScrollPosition.y - logListScrollPosition.y > logListLineHeight)
                {
                    scrollFollowMessages = false;
                }
            }

            float logLineX = collapseBadgeMaxWidth;

            // Render all the elements
            int firstRenderLogIndex = (int)(logListScrollPosition.y / logListLineHeight);
            int lastRenderLogIndex = firstRenderLogIndex + (int)(height / logListLineHeight);

            firstRenderLogIndex = Mathf.Clamp(firstRenderLogIndex, 0, this.renderList.Count);
            lastRenderLogIndex = Mathf.Clamp(lastRenderLogIndex, 0, this.renderList.Count);
            var buttonY = firstRenderLogIndex * this.logListLineHeight;

            for (int renderLogIndex = firstRenderLogIndex; renderLogIndex < lastRenderLogIndex; renderLogIndex++)
            {
                var countedLog = this.renderList[renderLogIndex];
                GUIStyle logLineStyle = (renderLogIndex % 2 == 0) ? entryStyleBackEven : entryStyleBackOdd;
                if (renderLogIndex == selectedLogIndex)
                {
                    GUI.backgroundColor = new Color(0.5f, 0.5f, 1);
                }
                else
                {
                    GUI.backgroundColor = Color.white;
                }

                // Make all messages single line
                var content = this.GetLogLineGUIContent(countedLog.Event);
                var drawRect = new Rect(logLineX, buttonY, contentRect.width, logListLineHeight);
                if (GUI.Button(drawRect, content, logLineStyle))
                {
                    // Select a message, or jump to source if it's double-clicked
                    if (renderLogIndex != selectedLogIndex)
                    {
                        selectedLogIndex = renderLogIndex;
                        selectedCallstackFrame = -1;
                    }
                }

                if (collapse)
                {
                    var collapseBadgeContent = new GUIContent(countedLog.Count.ToString());
                    var collapseBadgeSize = collapseBadgeStyle.CalcSize(collapseBadgeContent);
                    var collapseBadgeRect = new Rect(0, buttonY, collapseBadgeSize.x, collapseBadgeSize.y);
                    GUI.Button(collapseBadgeRect, collapseBadgeContent, collapseBadgeStyle);
                }

                buttonY += logListLineHeight;
            }

            // If we're following the log, move to the end
            if (scrollFollowMessages && this.renderList.Count > 0)
            {
                logListScrollPosition.y = ((this.renderList.Count + 1) * logListLineHeight) - scrollRect.height;
            }

            GUI.EndScrollView();
            drawPos.y += height;
            drawPos.x = 0;
            GUI.backgroundColor = oldColor;
        }

        private GUIContent GetLogLineGUIContent(NLogInterceptorEvent log)
        {
            string displayString = string.Format("{0:H:mm:ss.fff} [{1}] {2}", log.TimeStamp, log.LoggerName, log.Message);
            var content = new GUIContent(displayString, this.GetIconForLog(log));
            return content;
        }

        private Texture2D GetIconForLog(NLogInterceptorEvent log)
        {
            if (log.Level == LogLevel.Error)
            {
                return errorIcon;
            }

            if (log.Level == LogLevel.Warn)
            {
                return warningIcon;
            }

            return messageIcon;
        }

        private void DrawFilter()
        {
            Vector2 size;
            GuiUtils.LabelClamped(this.drawPos, "Filter Regex", GUI.skin.label, out size);
            drawPos.x += size.x;
            
            if (GuiUtils.ButtonClamped(this.drawPos, "Clear", GUI.skin.button, out size))
            {
                this.filterRegexText = null;
                this.filterRegex = null;

                GUIUtility.keyboardControl = 0;
                GUIUtility.hotControl = 0;
            }

            drawPos.x += size.x;

            var drawRect = new Rect(drawPos, new Vector2(position.width - drawPos.x, size.y));
            string newFilter = EditorGUI.TextArea(drawRect, this.filterRegexText);

            // If the filter has changed, invalidate our currently selected message
            if (this.filterRegexText != newFilter)
            {
                ClearSelectedMessage();
                this.filterRegexText = newFilter;
                this.filterRegex = new Regex(this.filterRegexText, RegexOptions.IgnoreCase);
                this.forceRepaint = true;
            }

            drawPos.y += size.y;
            drawPos.x = 0;
        }

        private class DrawingContext
        {
            public DrawingContext()
            {
                this.Target = new List<GUIContent>();
            }

            public NLogInterceptorEvent Event { get; set; }
            public List<GUIContent> Target { get; private set; }
            public GUIStyle Style { get; set; }
            public GUIStyle SourceStyle { get; set; }
            public float ContentHeight { get; set; }
            public float ContentWidth { get; set; }
            public float LineHeight { get; set; }
        }

        private void DrawStackDetails(DrawingContext context)
        {
            if (context.Event.StackTrace != null)
            {
                this.DrawStackTrace(context);
                return;
            }

            if (context.Event.Exception != null)
            {
                var typed = context.Event.Exception as UnityStackTraceException;
                if (typed != null)
                {
                    this.DrawUnityStackTrace(typed, context);
                    return;
                }
            }

            if (context.Event.ManualStack != null)
            {
                this.DrawManualStackTrace(context);
            }
        }

        private void DrawStackTraceLine(string line, DrawingContext context)
        {
            var content = new GUIContent(line);
            var contentSize = context.Style.CalcSize(content);
            context.ContentHeight += contentSize.y;
            context.LineHeight = Mathf.Max(context.LineHeight, contentSize.y);
            context.ContentWidth = Mathf.Max(contentSize.x, context.ContentWidth);
            context.Target.Add(content);
        }

        private void DrawManualStackTrace(DrawingContext context)
        {
            for (var i = 0; i < context.Event.ManualStack.Length; i++)
            {
                this.DrawStackTraceLine(context.Event.ManualStack[i], context);
            }
        }

        private void DrawUnityStackTrace(UnityStackTraceException exception, DrawingContext context)
        {
            string[] lines = exception.StackContents.Split(
                new[] {"\n"},
                StringSplitOptions.RemoveEmptyEntries);

            for (var i = 0; i < lines.Length; i++)
            {
                this.DrawStackTraceLine(lines[i], context);
            }
        }

        private void DrawStackTrace(DrawingContext context)
        {
            for (int i = 0; i < context.Event.StackTrace.FrameCount; i++)
            {
                var frame = context.Event.StackTrace.GetFrame(i);
                MethodBase method = frame.GetMethod();
                if (method == null || string.IsNullOrEmpty(method.Name))
                {
                    this.DrawStackTraceLine("<Unknown>", context);
                    continue;
                }

                string line = string.Format("[{0}] {1}.{2}()",
                    frame.GetILOffset().ToString().PadLeft(4, ' '),
                    method.ReflectedType == null ? "<Unknown>" : method.ReflectedType.FullName,
                    method.Name);
                
                this.DrawStackTraceLine(line, context);

                if (showFrameSource && i == selectedCallstackFrame)
                {
                    var sourceContent = new GUIContent(frame.ToString());
                    var sourceSize = context.SourceStyle.CalcSize(sourceContent);
                    context.ContentHeight += sourceSize.y;
                    context.ContentWidth = Mathf.Max(sourceSize.x, context.ContentWidth);
                }
            }
        }
        
        // The bottom of the panel - details of the selected log
        private void DrawLogDetails()
        {
            var oldColor = GUI.backgroundColor;

            this.selectedLogIndex = Mathf.Clamp(this.selectedLogIndex, 0, this.renderList.Count);

            if (this.renderList.Count > 0 && this.selectedLogIndex >= 0)
            {
                var countedLog = this.renderList[this.selectedLogIndex];
                
                var drawRect = new Rect(drawPos, new Vector2(position.width - drawPos.x, position.height - drawPos.y));

                this.drawingContext.Target.Clear();
                this.drawingContext.Event = countedLog.Event;
                this.drawingContext.ContentHeight = 0;
                this.drawingContext.ContentWidth = 0;
                this.drawingContext.LineHeight = 0;
                this.drawingContext.Style = entryStyleBackEven;
                this.drawingContext.SourceStyle = new GUIStyle(GUI.skin.textArea) { richText = true };

                // Work out the content we need to show, and the sizes
                this.DrawStackDetails(this.drawingContext);

                // Render the content
                var contentRect = new Rect(0, 0, Mathf.Max(this.drawingContext.ContentWidth, drawRect.width), this.drawingContext.ContentHeight);

                logDetailsScrollPosition = GUI.BeginScrollView(drawRect, logDetailsScrollPosition, contentRect);

                float lineY = 0;
                for (int i = 0; i < this.drawingContext.Target.Count; i++)
                {
                    var lineContent = this.drawingContext.Target[i];
                    if (lineContent != null)
                    {
                        this.drawingContext.Style = (i % 2 == 0) ? entryStyleBackEven : entryStyleBackOdd;
                        if (i == selectedCallstackFrame)
                        {
                            GUI.backgroundColor = new Color(0.5f, 0.5f, 1);
                        }
                        else
                        {
                            GUI.backgroundColor = Color.white;
                        }
                        
                        var lineRect = new Rect(0, lineY, contentRect.width, this.drawingContext.LineHeight);

                        // Handle clicks on the stack frame
                        if (GUI.Button(lineRect, lineContent, this.drawingContext.Style))
                        {
                            if (i == selectedCallstackFrame)
                            {
                                if (Event.current.button == 1)
                                {
                                    showFrameSource = !this.showFrameSource;
                                    Repaint();
                                }
                            }
                            else
                            {
                                selectedCallstackFrame = i;
                            }
                        }

                        lineY += this.drawingContext.LineHeight;

                        // Show the source code if needed
                        if (showFrameSource && i == selectedCallstackFrame)
                        {
                            GUI.backgroundColor = Color.white;

                            GUIContent sourceContent;
                            if (countedLog.Event.StackTrace != null)
                            {
                                var frame = countedLog.Event.StackTrace.GetFrame(i);
                                sourceContent = new GUIContent(frame.ToString());
                            }
                            else
                            {
                                sourceContent = new GUIContent(this.drawingContext.Target[i]);
                            }

                            var sourceSize = this.drawingContext.SourceStyle.CalcSize(sourceContent);
                            var sourceRect = new Rect(0, lineY, contentRect.width, sourceSize.y);

                            GUI.Label(sourceRect, sourceContent, this.drawingContext.SourceStyle);
                            lineY += sourceSize.y;
                        }
                    }
                }

                GUI.EndScrollView();
            }

            GUI.backgroundColor = oldColor;
        }
    }
}
