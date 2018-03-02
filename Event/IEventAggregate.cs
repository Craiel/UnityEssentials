namespace Assets.Scripts.Craiel.Essentials.Event
{
    public interface IEventAggregate
    {
        void Unsubscribe(BaseEventSubscriptionTicket ticket);
    }
}