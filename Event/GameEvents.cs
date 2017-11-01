namespace Assets.Scripts.Craiel.Essentials.Event
{
    using System;
    using Contracts;
    using Enums;
    using Essentials;
    using Scene;

    public class GameEvents : UnitySingletonBehavior<GameEvents>
    {
        private GameEventAggregate aggregate;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void Initialize()
        {
            this.RegisterInController(SceneObjectController.Instance, SceneRootCategory.System, true);

            base.Initialize();

            this.aggregate = new GameEventAggregate();
        }

        public GameEventSubscriptionTicket Subscribe<T>(GameEventAction<T> actionDelegate)
            where T : IGameEvent
        {
            return this.aggregate.Subscribe(actionDelegate);
        }

        public GameEventSubscriptionTicket Subscribe<T>(GameEventAction<T> actionDelegate, Func<T, bool> filterDelegate)
            where T : IGameEvent
        {
            return this.aggregate.Subscribe(actionDelegate, filterDelegate);
        }

        public void Unsubscribe(GameEventSubscriptionTicket ticket)
        {
            this.aggregate.Unsubscribe(ticket);
        }

        public void Send<T>(T eventData)
            where T : IGameEvent
        {
            this.aggregate.Send(eventData);
        }
    }
}
