namespace Craiel.UnityEssentials.Runtime.Event.Editor
{
    using System;
    using Contracts.Editor;

    public static class EditorEvents
    {
        private static readonly BaseEventAggregate<IEditorEvent> Aggregate = new BaseEventAggregate<IEditorEvent>();

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static BaseEventSubscriptionTicket Subscribe<TSpecific>(BaseEventAggregate<IEditorEvent>.GameEventAction<TSpecific> actionDelegate)
            where TSpecific : IEditorEvent
        {
            return Aggregate.Subscribe(actionDelegate);
        }

        public static BaseEventSubscriptionTicket Subscribe<TSpecific>(BaseEventAggregate<IEditorEvent>.GameEventAction<TSpecific> actionDelegate, Func<TSpecific, bool> filterDelegate)
            where TSpecific : IEditorEvent
        {
            return Aggregate.Subscribe(actionDelegate, filterDelegate);
        }

        public static void Unsubscribe(ref BaseEventSubscriptionTicket ticket)
        {
            Aggregate.Unsubscribe(ref ticket);
        }

        public static void Send<T>(T eventData)
            where T : IEditorEvent
        {
            Aggregate.Send(eventData);
        }
    }
}