namespace Craiel.UnityEssentials.Runtime.Event
{
    using System;
    using System.Runtime.CompilerServices;
    using Contracts;
    using Enums;
    using Scene;
    using Singletons;

    public class GameEvents : UnitySingletonBehavior<GameEvents>
    {
        private BaseEventAggregate<IGameEvent> aggregate;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void Initialize()
        {
            this.RegisterInController(SceneObjectController.Instance, SceneRootCategory.System, true);

            base.Initialize();

            this.aggregate = new BaseEventAggregate<IGameEvent>();
        }

        public static void Send<T>(T eventData)
            where T : IGameEvent
        {
            if (IsInstanceActive)
            {
                Instance.DoSend(eventData);
            }
        }

        public static void Subscribe<TSpecific>(BaseEventAggregate<IGameEvent>.GameEventAction<TSpecific> actionDelegate, 
            out BaseEventSubscriptionTicket ticket, 
            Func<TSpecific, bool> filterDelegate = null)
            where TSpecific : IGameEvent
        {
            ticket = null;
            if (IsInstanceActive)
            {
                ticket = Instance.DoSubscribe(actionDelegate, filterDelegate);
            }
        }
        
        public static void Unsubscribe(ref BaseEventSubscriptionTicket ticket)
        {
            if (IsInstanceActive)
            {
                Instance.DoUnsubscribe(ref ticket);
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void DoSend<T>(T eventData)
            where T : IGameEvent
        {
            this.aggregate.Send(eventData);
        }
        
        private BaseEventSubscriptionTicket DoSubscribe<TSpecific>(BaseEventAggregate<IGameEvent>.GameEventAction<TSpecific> actionDelegate, Func<TSpecific, bool> filterDelegate)
            where TSpecific : IGameEvent
        {
            return this.aggregate.Subscribe(actionDelegate, filterDelegate);
        }

        private void DoUnsubscribe(ref BaseEventSubscriptionTicket ticket)
        {
            this.aggregate.Unsubscribe(ref ticket);
        }
    }
}
