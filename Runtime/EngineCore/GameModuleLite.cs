namespace Craiel.UnityEssentials.Runtime.EngineCore
{
    using System.Collections.Generic;
    using Contracts;
    using Event;

    public abstract class GameModuleLite : IGameModule
    {
        private readonly IList<BaseEventSubscriptionTicket> managedEventSubscriptions;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        protected GameModuleLite()
        {
            this.managedEventSubscriptions = new List<BaseEventSubscriptionTicket>();
        }
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public virtual void Initialize()
        {
        }

        public virtual void Update()
        {
        }

        public virtual void FixedUpdate()
        {
        }

        public virtual void Destroy()
        {
            foreach (BaseEventSubscriptionTicket ticket in this.managedEventSubscriptions)
            {
                BaseEventSubscriptionTicket closure = ticket;
                GameEvents.Unsubscribe(ref closure);
            }
            
            this.managedEventSubscriptions.Clear();
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected void SubscribeEvent<TE>(BaseEventAggregate<IGameEvent>.GameEventAction<TE> callback)
            where TE : IGameEvent
        {
            GameEvents.Subscribe(callback, out BaseEventSubscriptionTicket ticket);
            
            this.managedEventSubscriptions.Add(ticket);
        }
    }
}