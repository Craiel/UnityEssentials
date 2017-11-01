namespace Assets.Scripts.Craiel.Essentials
{
    using System.Threading;

    public class UnitySynchronizationContext : SynchronizationContext
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void Post(SendOrPostCallback d, object state)
        {
            UnitySynchronizationDispatcher.Instance.InvokeLater(() => { d.Invoke(state); });
        }

        public override void Send(SendOrPostCallback d, object state)
        {
            UnitySynchronizationDispatcher.Instance.InvokeLater(() => { d.Invoke(state); });
        }
    }
}
