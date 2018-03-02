namespace Assets.Scripts.Craiel.Essentials.Event
{
    using System;
    using System.Collections.Generic;
    
    public class BaseEventAggregate<T> : IEventAggregate
        where T : class
    {
        private readonly IDictionary<Type, BaseEventTargetCollection<T>> subscribers;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public BaseEventAggregate()
        {
            this.subscribers = new Dictionary<Type, BaseEventTargetCollection<T>>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public delegate void GameEventAction<in TSpecific>(TSpecific eventData)
            where TSpecific : T;
        
        public BaseEventSubscriptionTicket Subscribe<TSpecific>(GameEventAction<TSpecific> actionDelegate)
            where TSpecific : T
        {
            var ticket = new BaseEventSubscriptionTicket(this, typeof(TSpecific), actionDelegate);
            this.DoSubscribe(ticket);
            return ticket;
        }

        public BaseEventSubscriptionTicket Subscribe<TSpecific>(GameEventAction<TSpecific> actionDelegate, Func<TSpecific, bool> filterDelegate)
            where TSpecific : T
        {
            var ticket = new BaseEventSubscriptionTicket(this, typeof(TSpecific), actionDelegate)
                {
                    FilterDelegate = x => filterDelegate((TSpecific) x)
                };

            this.DoSubscribe(ticket);
            return ticket;
        }

        public void Unsubscribe(BaseEventSubscriptionTicket ticket)
        {
            this.DoUnsubscribe(ticket);
        }

        public void Send<TSpecific>(TSpecific eventData)
            where TSpecific : T
        {
            this.DoSend<TSpecific>(eventData);
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected void DoSubscribe(BaseEventSubscriptionTicket ticket)
        {
            lock (this.subscribers)
            {
                BaseEventTargetCollection<T> targets;
                if (!this.subscribers.TryGetValue(ticket.TargetType, out targets))
                {
                    targets = new BaseEventTargetCollection<T>();
                    this.subscribers.Add(ticket.TargetType, targets);
                }

                targets.Add(ticket);
            }
        }

        protected void DoUnsubscribe(BaseEventSubscriptionTicket ticket)
        {
            lock (this.subscribers)
            {
                BaseEventTargetCollection<T> targets;
                if (this.subscribers.TryGetValue(ticket.TargetType, out targets))
                {
                    if (!targets.Remove(ticket))
                    {
                        throw new InvalidOperationException("Unsubscribe coult not find the given target");
                    }
                }
            }
        }

        protected void DoSend<TSpecific>(TSpecific eventData)
            where TSpecific : T
        {
            lock (this.subscribers)
            {
                BaseEventTargetCollection<T> targets;
                if (this.subscribers.TryGetValue(typeof(TSpecific), out targets))
                {
                    targets.Send(eventData);
                }
            }
        }
    }
}
