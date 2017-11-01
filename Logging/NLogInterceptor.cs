namespace Assets.Scripts.Craiel.Essentials.Logging
{
    using System;
    using System.Collections.Generic;
    using Essentials;
    using NLog;

    public class NLogInterceptor : UnitySingleton<NLogInterceptor>
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public event Action<NLogInterceptorEvent> OnLogChanged;

        public NLogInterceptTarget Target { get; private set; }

        public bool PauseOnError { get; set; }

        public IList<NLogInterceptorEvent> Events { get; private set; }

        public IDictionary<LogLevel, int> Count { get; private set; }

        public IList<string> Names { get; private set; }
        
        public override void Initialize()
        {
            base.Initialize();

            this.Events = new List<NLogInterceptorEvent>();
            this.Count = new Dictionary<LogLevel, int>();
            this.Names = new List<string>();

            this.Target = new NLogInterceptTarget();
            this.Target.OnEventReceived += this.OnEventReceived;

            UnityEngine.Debug.Log("NLog Interceptor Initialized");
        }

        public void Clear()
        {
            this.Events.Clear();
            this.Count.Clear();
            this.Names.Clear();
        }

        public int GetCount(LogLevel level)
        {
            int result;
            if (this.Count != null && this.Count.TryGetValue(level, out result))
            {
                return result;
            }

            return 0;
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void OnEventReceived(LogEventInfo @event)
        {
            var eventData = new NLogInterceptorEvent(@event);
            this.Events.Add(eventData);

            if (!this.Count.ContainsKey(eventData.Level))
            {
                this.Count.Add(eventData.Level, 1);
            }
            else
            {
                this.Count[eventData.Level]++;
            }

            if (eventData.Level == LogLevel.Error && this.PauseOnError)
            {
                UnityEngine.Debug.Break();
            }

            if (!this.Names.Contains(eventData.LoggerName))
            {
                this.Names.Add(eventData.LoggerName);
            }

            if (this.OnLogChanged != null)
            {
                this.OnLogChanged(eventData);
            }
        }
    }
}
