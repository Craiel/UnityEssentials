namespace Craiel.UnityEssentialsUI.Runtime
{
    using System.Collections.Generic;
    using Events;
    using UnityEngine;
    using UnityEssentials.Runtime.Contracts;
    using UnityEssentials.Runtime.EngineCore;
    using UnityEssentials.Runtime.Event;
    using UnityEssentials.Runtime.Events;

    public class UIEngineBehavior : MonoBehaviour
    {
        private readonly IList<BaseEventSubscriptionTicket> managedEventSubscriptions;
        
        private bool isInitialized;
        
        private BaseEventSubscriptionTicket initializationEventTicket;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        protected UIEngineBehavior()
        {
            this.managedEventSubscriptions = new List<BaseEventSubscriptionTicket>();
        }
        
        public virtual void Awake()
        {
#if DEBUG
            EssentialCoreUI.Logger.Info("UIEngineBehavior.Awake: {0}", this.GetType().Name);
#endif
        }
        
        public virtual void OnDestroy()
        {
#if DEBUG
            EssentialCoreUI.Logger.Info("UIEngineBehavior.Destroy: {0}", this.GetType().Name);
#endif
            
            foreach (BaseEventSubscriptionTicket ticket in this.managedEventSubscriptions)
            {
                BaseEventSubscriptionTicket closure = ticket;
                GameEvents.Unsubscribe(ref closure);
            }

            this.managedEventSubscriptions.Clear();
        }
        
        public virtual void Update()
        {
            if (!this.isInitialized)
            {
                if (EssentialEngineState.IsInitialized)
                {
                    // Immediately initialize, the engine is already done
                    this.Initialize();
                }
                else
                {
                    if (!GameEvents.IsInstanceActive)
                    {
                        // GameEvents is not yet available, can not complete initialization at this time
                        return;
                    }
                    
                    GameEvents.Subscribe<EventEngineInitialized>(this.OnEngineInitialized, out this.initializationEventTicket);
                }

                // We are now initialized, give an extra frame break to the next update loop
                this.isInitialized = true;
                return;
            }
        }
        
        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected void SubscribeEvent<T>(BaseEventAggregate<IGameEvent>.GameEventAction<T> callback)
            where T : IGameEvent
        {
            BaseEventSubscriptionTicket ticket;
            GameEvents.Subscribe(callback, out ticket);
            this.managedEventSubscriptions.Add(ticket);
        }
        
        protected virtual void Initialize()
        {
#if DEBUG
            EssentialCoreUI.Logger.Info("UIEngineBehavior.Initialize: {0}", this.GetType().Name);
#endif
        }
        
        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void OnEngineInitialized(EventEngineInitialized eventData)
        {
            GameEvents.Unsubscribe(ref this.initializationEventTicket);
            
            this.Initialize();
        }
    }
}