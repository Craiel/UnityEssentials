namespace Craiel.UnityEssentials.Runtime.Event
{
    public interface IEventAggregate
    {
        void Unsubscribe(ref BaseEventSubscriptionTicket ticket);
    }
}