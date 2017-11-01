namespace Assets.Scripts.Craiel.Essentials.Event
{
    using System;
    using System.Collections.Generic;
    using Contracts;

    public delegate void GameEventAction<in T>(T eventData) where T : IGameEvent;

    public class GameEventAggregate
    {
        private readonly IDictionary<Type, GameEventTargetCollection> subscribers;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public GameEventAggregate()
        {
            this.subscribers = new Dictionary<Type, GameEventTargetCollection>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public GameEventSubscriptionTicket Subscribe<T>(GameEventAction<T> actionDelegate)
            where T : IGameEvent
        {
            var ticket = new GameEventSubscriptionTicket(this, typeof(T), actionDelegate);
            this.DoSubscribe(ticket);
            return ticket;
        }

        public GameEventSubscriptionTicket Subscribe<T>(GameEventAction<T> actionDelegate, Func<T, bool> filterDelegate)
            where T : IGameEvent
        {
            var ticket = new GameEventSubscriptionTicket(this, typeof(T), actionDelegate)
                {
                    FilterDelegate = x => filterDelegate((T) x)
                };

            this.DoSubscribe(ticket);
            return ticket;
        }

        public void Unsubscribe(GameEventSubscriptionTicket ticket)
        {
            this.DoUnsubscribe(ticket);
        }

        public void Send<T>(T eventData)
            where T : IGameEvent
        {
            this.DoSend(typeof(T), eventData);
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected void DoSubscribe(GameEventSubscriptionTicket ticket)
        {
            lock (this.subscribers)
            {
                GameEventTargetCollection targets;
                if (!this.subscribers.TryGetValue(ticket.TargetType, out targets))
                {
                    targets = new GameEventTargetCollection();
                    this.subscribers.Add(ticket.TargetType, targets);
                }

                targets.Add(ticket);
            }
        }

        protected void DoUnsubscribe(GameEventSubscriptionTicket ticket)
        {
            lock (this.subscribers)
            {
                GameEventTargetCollection targets;
                if (this.subscribers.TryGetValue(ticket.TargetType, out targets))
                {
                    if (!targets.Remove(ticket))
                    {
                        throw new InvalidOperationException("Unsubscribe coult not find the given target");
                    }
                }
            }
        }

        protected void DoSend<T>(Type eventType, T eventData)
            where T : IGameEvent
        {
            lock (this.subscribers)
            {
                GameEventTargetCollection targets;
                if (this.subscribers.TryGetValue(eventType, out targets))
                {
                    targets.Send(eventData);
                }
            }
        }
    }
}
