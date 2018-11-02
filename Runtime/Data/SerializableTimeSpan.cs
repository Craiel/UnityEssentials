namespace Craiel.UnityEssentials.Runtime.Data
{
    using System;
    using UnityEngine;

    [Serializable]
    public class SerializableTimeSpan : ISerializationCallbackReceiver
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [HideInInspector]
        public TimeSpan TimeSpan;
        
        [HideInInspector]
        [SerializeField]
        public int Days;
        
        [HideInInspector]
        [SerializeField]
        public int Hours;
        
        [HideInInspector]
        [SerializeField]
        public int Minutes;
        
        [HideInInspector]
        [SerializeField]
        public int Seconds;
        
        [HideInInspector]
        [SerializeField]
        public int Milliseconds;

        public double TotalSeconds
        {
            get { return this.TimeSpan.TotalSeconds; }
        }
        
        public static implicit operator TimeSpan(SerializableTimeSpan source) 
        {
            return source.TimeSpan;
        }

        public static implicit operator SerializableTimeSpan(TimeSpan source) 
        {
            return new SerializableTimeSpan {TimeSpan = source};
        }

        public void OnAfterDeserialize()
        {
            this.TimeSpan = new TimeSpan(this.Days, this.Hours, this.Minutes, this.Seconds, this.Milliseconds);
        }

        public void OnBeforeSerialize()
        {
            this.Days = this.TimeSpan.Days;
            this.Hours = this.TimeSpan.Hours;
            this.Minutes = this.TimeSpan.Minutes;
            this.Seconds = this.TimeSpan.Seconds;
            this.Milliseconds = this.TimeSpan.Milliseconds;
        }
    }
}