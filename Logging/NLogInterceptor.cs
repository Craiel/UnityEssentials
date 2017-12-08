namespace Assets.Scripts.Craiel.Essentials.Logging
{
    using System;
    using System.Collections.Generic;
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
            var interceptorEvent = new NLogInterceptorEvent(@event);
            this.Events.Add(interceptorEvent);

            if (!this.Count.ContainsKey(@event.Level))
            {
                this.Count.Add(@event.Level, 1);
            }
            else
            {
                this.Count[@event.Level]++;
            }

            if (@event.Level == LogLevel.Error && this.PauseOnError)
            {
                UnityEngine.Debug.Break();
            }

            if (!this.Names.Contains(@event.LoggerName))
            {
                this.Names.Add(@event.LoggerName);
            }

            if (this.OnLogChanged != null)
            {
                this.OnLogChanged(interceptorEvent);
            }
        }
    }
}
