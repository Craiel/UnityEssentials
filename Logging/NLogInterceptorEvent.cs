namespace Craiel.UnityEssentials.Logging
{
    using System;
    using System.Diagnostics;
    using NLog;

    public class NLogInterceptorEvent
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public NLogInterceptorEvent(LogEventInfo info)
        {
            this.LoggerName = info.LoggerName;
            this.Level = info.Level;
            this.StackTrace = info.StackTrace;
            this.Exception = info.Exception;
            this.TimeStamp = info.TimeStamp;

            string[] messageSegments = info.FormattedMessage.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (messageSegments.Length > 1)
            {
                this.Message = messageSegments[0];
                this.ManualStack = messageSegments;
            }
            else
            {
                this.Message = info.FormattedMessage;
            }
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public DateTime TimeStamp { get; private set; }

        public string LoggerName { get; private set; }

        public string Message { get; private set; }

        public LogLevel Level { get; private set; }

        public StackTrace StackTrace { get; private set; }

        public Exception Exception { get; private set; }

        public string[] ManualStack { get; private set; }
    }
}
