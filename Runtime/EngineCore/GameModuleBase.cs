namespace Craiel.UnityEssentials.Runtime.EngineCore
{
    using System.Collections.Generic;
    using Contracts;
    using Event;

    public class GameModuleBase<T> : IGameModule
        where T: GameModuleCore<T>
    {
        private readonly IList<BaseEventSubscriptionTicket> managedEventSubscriptions;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        protected GameModuleBase(T parent)
        {
            this.Parent = parent;
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
        protected T Parent;

        protected void SubscribeEvent<T>(BaseEventAggregate<IGameEvent>.GameEventAction<T> callback)
            where T : IGameEvent
        {
            GameEvents.Subscribe(callback, out BaseEventSubscriptionTicket ticket);
            
            this.managedEventSubscriptions.Add(ticket);
        }
    }
}