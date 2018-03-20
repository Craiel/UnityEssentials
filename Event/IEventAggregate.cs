namespace Craiel.UnityEssentials.Event
{
    public interface IEventAggregate
    {
        void Unsubscribe(ref BaseEventSubscriptionTicket ticket);
    }
}