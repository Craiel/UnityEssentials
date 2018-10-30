namespace Craiel.UnityEssentials.Runtime.Data
{
    using System;
    using UnityEngine;

    [Serializable]
    public class SerializableDateTime : ISerializationCallbackReceiver
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [HideInInspector]
        public DateTime DateTime;

        [HideInInspector]
        [SerializeField]
        public bool UseDate = true;

        [HideInInspector]
        [SerializeField]
        public bool UseTime = true;
        
        [HideInInspector]
        [SerializeField]
        public int Year;
        
        [HideInInspector]
        [SerializeField]
        public int Month;
        
        [HideInInspector]
        [SerializeField]
        public int Day;
        
        [HideInInspector]
        [SerializeField]
        public int Hour;
        
        [HideInInspector]
        [SerializeField]
        public int Minute;
        
        [HideInInspector]
        [SerializeField]
        public int Second;
        
        [HideInInspector]
        [SerializeField]
        public int Millisecond;

        [HideInInspector]
        [SerializeField]
        public DateTimeKind Kind = DateTimeKind.Utc;
        
        public static implicit operator DateTime(SerializableDateTime source) 
        {
            return source.DateTime;
        }

        public static implicit operator SerializableDateTime(DateTime source) 
        {
            return new SerializableDateTime {DateTime = source};
        }

        public void OnAfterDeserialize()
        {
            this.DateTime = new DateTime(this.Year, this.Month, this.Day, this.Hour, this.Minute, this.Second, this.Millisecond, this.Kind);
        }

        public void OnBeforeSerialize() 
        {
            this.Year = this.DateTime.Year;
            this.Month = this.DateTime.Month;
            this.Day = this.DateTime.Day;
            this.Hour = this.DateTime.Hour;
            this.Minute = this.DateTime.Minute;
            this.Second = this.DateTime.Second;
            this.Kind = this.DateTime.Kind;
        }
    }
}