namespace Craiel.UnityEssentials.Event
{
    using System;

    public class BaseEventSubscriptionTicket
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public BaseEventSubscriptionTicket(Type targetType, object targetDelegate)
        {
            this.TargetType = targetType;
            this.TargetDelegate = targetDelegate;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public object TargetDelegate { get; private set; }

        public Type TargetType { get; private set; }

        public Func<object, bool> FilterDelegate { get; set; }
    }
}
