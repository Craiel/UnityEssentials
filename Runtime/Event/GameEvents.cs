namespace Craiel.UnityEssentials.Runtime.Event
{
    using System;
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

        public BaseEventSubscriptionTicket Subscribe<TSpecific>(BaseEventAggregate<IGameEvent>.GameEventAction<TSpecific> actionDelegate)
            where TSpecific : IGameEvent
        {
            return this.aggregate.Subscribe(actionDelegate);
        }

        public BaseEventSubscriptionTicket Subscribe<TSpecific>(BaseEventAggregate<IGameEvent>.GameEventAction<TSpecific> actionDelegate, Func<TSpecific, bool> filterDelegate)
            where TSpecific : IGameEvent
        {
            return this.aggregate.Subscribe(actionDelegate, filterDelegate);
        }

        public void Unsubscribe(ref BaseEventSubscriptionTicket ticket)
        {
            this.aggregate.Unsubscribe(ref ticket);
        }

        public void Send<T>(T eventData)
            where T : IGameEvent
        {
            this.aggregate.Send(eventData);
        }
    }
}
