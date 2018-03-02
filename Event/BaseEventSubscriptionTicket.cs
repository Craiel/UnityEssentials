namespace Assets.Scripts.Craiel.Essentials.Event
{
    using System;

    public class BaseEventSubscriptionTicket : IDisposable
    {
        private IEventAggregate aggregate;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public BaseEventSubscriptionTicket(IEventAggregate aggregate, Type targetType, object targetDelegate)
        {
            this.aggregate = aggregate;
            this.TargetType = targetType;
            this.TargetDelegate = targetDelegate;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public object TargetDelegate { get; private set; }

        public Type TargetType { get; private set; }

        public Func<object, bool> FilterDelegate { get; set; }

        public void Dispose()
        {
            this.Dispose(true);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                this.aggregate.Unsubscribe(this);
                this.aggregate = null;
                this.TargetType = null;
                this.TargetDelegate = null;

                GC.SuppressFinalize(this);
            }
        }
    }
}
