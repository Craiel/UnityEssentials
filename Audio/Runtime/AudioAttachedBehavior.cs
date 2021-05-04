namespace Craiel.UnityAudio.Runtime
{
    using UnityEssentials.Runtime.Event;
    using UnityEssentialsUI.Runtime;
    using UnityGameData.Runtime;
    using UnityGameData.Runtime.Events;

    public abstract class AudioAttachedBehavior : UIEngineBehavior
    {
        private BaseEventSubscriptionTicket loadEventTicket;
        private BaseEventSubscriptionTicket unloadEventTicket;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool DataLoaded { get; private set; }

        public override void OnDestroy()
        {
            GameEvents.Unsubscribe(ref this.loadEventTicket);
            GameEvents.Unsubscribe(ref this.unloadEventTicket);

            this.StopAllAudio();
            
            base.OnDestroy();
        }

        public abstract void StopAllAudio();

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void Initialize()
        {
            base.Initialize();
            
            GameEvents.Subscribe<EventGameDataLoaded>(this.OnGameDataLoaded, out this.loadEventTicket);
            GameEvents.Subscribe<EventGameDataUnloaded>(this.OnGameDataUnloaded, out this.unloadEventTicket);

            if (GameRuntimeData.Instance.IsLoaded)
            {
                // Data is already loaded so call setup immediatly
                // We keep the event ticket in case the data gets reloaded
                this.SetupAudio();
                this.DataLoaded = true;
            }
        }
        
        protected virtual void SetupAudio()
        {
            this.StopAllAudio();
        }

        protected virtual void ReleaseAudio()
        {
            this.StopAllAudio();
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void OnGameDataLoaded(EventGameDataLoaded eventData)
        {
            this.SetupAudio();
            this.DataLoaded = true;
        }

        private void OnGameDataUnloaded(EventGameDataUnloaded eventData)
        {
            this.ReleaseAudio();
            this.DataLoaded = false;
        }
    }
}
