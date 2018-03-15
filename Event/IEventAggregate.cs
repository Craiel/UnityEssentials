namespace Craiel.UnityEssentials.Event
{
    public interface IEventAggregate
    {
        void Unsubscribe(BaseEventSubscriptionTicket ticket);
    }
}