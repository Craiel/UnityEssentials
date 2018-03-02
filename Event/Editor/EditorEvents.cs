﻿namespace Assets.Scripts.Craiel.Essentials.Event.Editor
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

        public static BaseEventSubscriptionTicket Subscribe<TSpecific>(BaseEventAggregate<IEditorEvent>.GameEventAction<TSpecific> actionDelegate, Func<IEditorEvent, bool> filterDelegate)
            where TSpecific : IEditorEvent
        {
            return Aggregate.Subscribe(actionDelegate, filterDelegate);
        }

        public static void Unsubscribe(BaseEventSubscriptionTicket ticket)
        {
            Aggregate.Unsubscribe(ticket);
        }

        public static void Send<T>(T eventData)
            where T : IEditorEvent
        {
            Aggregate.Send(eventData);
        }
    }
}