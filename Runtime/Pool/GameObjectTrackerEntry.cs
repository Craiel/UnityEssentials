namespace Craiel.UnityEssentials.Runtime.Pool
{
    using System;
    using UnityEngine;

    public class GameObjectTrackerEntry
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public GameObjectTrackerEntry(GameObjectTrackerTicket ticket, GameObject target, byte trackers)
        {
            this.Ticket = ticket;
            this.Object = new WeakReference<GameObject>(target);
            this.TrackerIndex = new bool[trackers];
            this.AliveTime = Time.time;
        }
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        
        // Note: Fields for Performance
        public readonly GameObjectTrackerTicket Ticket;
        public readonly WeakReference<GameObject> Object;
        public readonly bool[] TrackerIndex;
        public float AliveTime;
    }
}