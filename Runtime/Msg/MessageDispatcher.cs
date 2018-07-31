namespace Craiel.UnityEssentials.Runtime.Msg
{
    using System;
    using System.Collections.Generic;
    using Collections;
    using Contracts;
    using Enums;
    using Pool;
    using UnityEngine;

    /// <summary>
    /// A <see cref="MessageDispatcher"/> is in charge of the creation, dispatch, and management of telegrams
    /// </summary>
    public class MessageDispatcher : ITelegraph
    {
        private static readonly MessageDispatcherPool Pool = new MessageDispatcherPool();

        private readonly PriorityQueue<Telegram> queue;

        private readonly IDictionary<int, IList<ITelegraph>> messageListeners;

        private readonly IDictionary<int, IList<ITelegramProvider>> messageProviders;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------

        /// <summary>
        /// Creates a new <see cref="MessageDispatcher"/>
        /// </summary>
        public MessageDispatcher()
        {
            this.queue = new PriorityQueue<Telegram>();
            this.messageListeners = new Dictionary<int, IList<ITelegraph>>();
            this.messageProviders = new Dictionary<int, IList<ITelegramProvider>>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------

        /// <summary>
        /// Sets debug mode on/off
        /// </summary>
        public bool DebugEnabled { get; set; }

        /// <summary>
        /// Registers a listener for the specified message code. 
        /// Messages without an explicit receiver are broadcasted to all its registered listeners.
        /// </summary>
        /// <param name="listener">the listener to add</param>
        /// <param name="message">the message code</param>
        public void AddListener(ITelegraph listener, int message)
        {
            IList<ITelegraph> listeners;
            if (!this.messageListeners.TryGetValue(message, out listeners))
            {
                listeners = new List<ITelegraph>();
                this.messageListeners.Add(message, listeners);
            }

            listeners.Add(listener);

            // Dispatch messages from registered providers
            IList<ITelegramProvider> providers;
            if (this.messageProviders.TryGetValue(message, out providers))
            {
                for (int i = 0, n = providers.Count; i < n; i++)
                {
                    ITelegramProvider provider = providers[i];
                    object info = provider.ProvideMessageInfo(message, listener);
                    if (info != null)
                    {
                        ITelegraph sender = info as ITelegraph;
                        this.DispatchMessage(message, 0, sender, listener, info);
                    }
                }
            }
        }

        /// <summary>
        /// Registers a listener for the specified message code. 
        /// Messages without an explicit receiver are broadcasted to all its registered listeners.
        /// </summary>
        /// <param name="listener">the listener to add</param>
        /// <param name="messages">the message codes</param>
        public void AddListener(ITelegraph listener, params int[] messages)
        {
            foreach (int message in messages)
            {
                this.AddListener(listener, message);
            }
        }

        /// <summary>
        /// Registers a provider for the specified message code
        /// </summary>
        /// <param name="provider">the message code</param>
        /// <param name="message">the provider to add</param>
        public void AddProvider(ITelegramProvider provider, int message)
        {
            IList<ITelegramProvider> providers;
            if (!this.messageProviders.TryGetValue(message, out providers))
            {
                providers = new List<ITelegramProvider>();
                this.messageProviders.Add(message, providers);
            }

            providers.Add(provider);
        }

        /// <summary>
        /// Registers a provider for a selection of message types
        /// </summary>
        /// <param name="provider">the provider to add</param>
        /// <param name="messages">the message codes</param>
        public void AddProvider(ITelegramProvider provider, params int[] messages)
        {
            foreach (int message in messages)
            {
                this.AddProvider(provider, message);
            }
        }

        /// <summary>
        /// Unregister the specified listener for the specified message code
        /// </summary>
        /// <param name="listener">the listener to remove</param>
        /// <param name="message">the message code</param>
        public void RemoveListener(ITelegraph listener, int message)
        {
            IList<ITelegraph> listeners;
            if (this.messageListeners.TryGetValue(message, out listeners))
            {
                listeners.Remove(listener);
            }
        }

        /// <summary>
        /// Unregister the specified listener for the selection of message codes
        /// </summary>
        /// <param name="listener">the listener to remove</param>
        /// <param name="messages">the message codes</param>
        public void RemoveListener(ITelegraph listener, params int[] messages)
        {
            foreach (int message in messages)
            {
                this.RemoveListener(listener, message);
            }
        }

        /// <summary>
        /// Unregisters all the listeners for the specified message code
        /// </summary>
        /// <param name="message">the message code</param>
        public void ClearListeners(int message)
        {
            this.messageListeners.Remove(message);
        }

        /// <summary>
        /// Unregisters all the listeners for the given message codes
        /// </summary>
        /// <param name="messages">the message codes</param>
        public void ClearListeners(params int[] messages)
        {
            foreach (int message in messages)
            {
                this.ClearListeners(message);
            }
        }

        /// <summary>
        /// Removes all the registered listeners for all the message codes
        /// </summary>
        public void ClearListeners()
        {
            this.messageListeners.Clear();
        }

        /// <summary>
        /// Unregisters all the providers for the specified message code
        /// </summary>
        /// <param name="message">the message code</param>
        public void ClearProviders(int message)
        {
            this.messageProviders.Remove(message);
        }

        /// <summary>
        /// Unregisters all the providers for the given message codes
        /// </summary>
        /// <param name="messages">the message codes</param>
        public void ClearProviders(params int[] messages)
        {
            foreach (int message in messages)
            {
                this.ClearProviders(message);
            }
        }

        /// <summary>
        /// Removes all the registered providers for all the message codes
        /// </summary>
        public void ClearProviders()
        {
            this.messageProviders.Clear();
        }

        /// <summary>
        /// Removes all the telegrams from the queue and releases them to the internal pool
        /// </summary>
        public void ClearQueue()
        {
            for (int i = 0; i < this.queue.Size; i++)
            {
                Pool.Free(this.queue.Get(i));
            }

            this.queue.Clear();
        }

        /// <summary>
        /// Removes all the telegrams from the queue and the registered listeners for all the messages
        /// </summary>
        public void Clear()
        {
            this.ClearQueue();
            this.ClearListeners();
            this.ClearProviders();
        }
        
        /// <summary>
        /// Given a message, a receiver, a sender and any time delay, this method routes the message to the correct agents (if no delay)
        ///  or stores in the message queue to be dispatched at the correct time
        /// </summary>
        /// <param name="message">the message code</param>
        /// <param name="delay">the delay in seconds</param>
        /// <param name="sender">the sender of the telegram</param>
        /// <param name="receiver">the receiver of the telegram; if it's <code>null</code> the telegram is broadcasted to all the receivers registered for the specified message code</param>
        /// <param name="extraInfo">an optional object</param>
        /// <param name="needReturnReceipt">whether the return receipt is needed or not</param>
        public void DispatchMessage(
            int message,
            float delay = 0f,
            ITelegraph sender = null,
            ITelegraph receiver = null,
            object extraInfo = null,
            bool needReturnReceipt = false)
        {
            if (sender == null && needReturnReceipt)
            {
                throw new ArgumentException("Sender cannot be null when a return receipt is needed");
            }

            // Get a telegram from the pool
            Telegram telegram = Pool.Obtain();
            telegram.Sender = sender;
            telegram.Receiver = receiver;
            telegram.Message = message;
            telegram.ExtraInfo = extraInfo;
            telegram.ReturnReceiptStatus = needReturnReceipt ? TelegramReturnReceiptStatus.Needed : TelegramReturnReceiptStatus.Unneeded;

            // If there is no delay, route telegram immediately
            if (delay <= 0.0f)
            {
                // TODO: should we set the timestamp here?
                // telegram.Timestamp = GDXAI.TimePiece.Time;

                if (this.DebugEnabled)
                {
                    float currentTime = Time.time;
                    EssentialsCore.Logger.Info("Instant telegram dispatched at time: {0} by {1} for {2}. Message code is " + message, currentTime, sender, receiver);
                }

                // Send the telegram to the recipient
                this.Discharge(telegram);
            }
            else
            {
                float currentTime = Time.time;

                // Set the timestamp for the delayed telegram
                telegram.Timestamp = currentTime + delay;

                // Put the telegram in the queue
                bool added = this.queue.Add(telegram);

                // Return it to the pool if has been rejected
                if (!added)
                {
                    Pool.Free(telegram);
                }

                if (this.DebugEnabled)
                {
                    if (added)
                    {
                        EssentialsCore.Logger.Info("Delayed telegram from {0} for {1} recorded at time {2}. Message code is {3}", sender, receiver, currentTime, message);
                    }
                    else
                    {
                        EssentialsCore.Logger.Info("Delayed telegram from {0} for {1} rejected by the queue. Message code is {2}", sender, receiver, message);
                    }
                }
            }
        }

        /// <summary>
        /// Dispatches any delayed telegrams with a timestamp that has expired. Dispatched telegrams are removed from the queue.
        /// <para>
        /// This method must be called regularly from inside the main game loop to facilitate the correct and timely dispatch of any
        /// delayed messages.Notice that the message dispatcher internally calls <see cref="GDXAI.TimePiece"/>
        /// to get the current AI time and properly dispatch delayed messages.
        /// This means that:
        ///  - if you forget to <see cref="ITimePiece.Update"/> the timepiece the delayed messages won't be dispatched.
        ///  - ideally the timepiece should be updated before the message dispatcher.
        /// </para>
        /// </summary>
        public void Update()
        {
            // Peek at the queue to see if any telegrams need dispatching.
            // Remove all telegrams from the front of the queue that have gone
            // past their time stamp.
            Telegram telegram;
            while ((telegram = this.queue.Peek()) != null)
            {
                // Exit loop if the telegram is in the future
                if (telegram.Timestamp > Time.time)
                {
                    break;
                }

                if (this.DebugEnabled)
                {
                    EssentialsCore.Logger.Info("Queued telegram ready for dispatch: Sent to {0}. Message code is {1}", telegram.Receiver, telegram.Message);
                }
                
                // Send the telegram to the recipient
                this.Discharge(telegram);

                // Remove it from the queue
                this.queue.Poll();
            }
        }

        /// <summary>
        /// Scans the queue and passes pending messages to the given callback in any particular order.
        /// <para>
        /// Typically this method is used to save (serialize) pending messages and restore (deserialize and schedule) them back on game* loading
        /// </para>
        /// </summary>
        /// <param name="callback">the callback to pass the messages to</param>
        public void ScanQueue(IPendingMessageCallback callback)
        {
            int queueSize = this.queue.Size;
            for (int i = 0; i < queueSize; i++)
            {
                Telegram telegram = this.queue.Get(i);
                callback.Report(
                    telegram.Timestamp - Time.time,
                    telegram.Sender,
                    telegram.Receiver,
                    telegram.Message,
                    telegram.ExtraInfo,
                    telegram.ReturnReceiptStatus);
            }
        }

        /// <summary>
        /// This method is used by <see cref="DispatchMessage"/> for immediate telegrams and
        /// <see cref="Update"/> for delayed telegrams. It first calls the message handling method of the
        /// receiving agents with the specified telegram then returns the telegram to the pool.
        /// </summary>
        /// <param name="telegram">the telegram to discharge</param>
        public void Discharge(Telegram telegram)
        {
            if (telegram.Receiver != null)
            {
                // Dispatch the telegram to the receiver specified by the telegram itself
                if (!telegram.Receiver.HandleMessage(telegram))
                {
                    // Telegram could not be handled
                    if (this.DebugEnabled)
                    {
                        EssentialsCore.Logger.Info("Message {0} not handled", telegram.Message);
                    }
                }
            }
            else
            {
                // Dispatch the telegram to all the registered receivers
                int handledCount = 0;
                IList<ITelegraph> listeners;
                if (this.messageListeners.TryGetValue(telegram.Message, out listeners))
                {
                    foreach (ITelegraph listener in listeners)
                    {
                        if (listener.HandleMessage(telegram))
                        {
                            handledCount++;
                        }
                    }
                }

                // Telegram could not be handled
                if (this.DebugEnabled && handledCount == 0)
                {
                    EssentialsCore.Logger.Info("Message {0} not handled", telegram.Message);
                }
            }

            if (telegram.ReturnReceiptStatus == TelegramReturnReceiptStatus.Needed)
            {
                // Use this telegram to send the return receipt
                telegram.Receiver = telegram.Sender;
                telegram.Sender = this;
                telegram.ReturnReceiptStatus = TelegramReturnReceiptStatus.Sent;
                this.Discharge(telegram);
            }
            else
            {
                // Release the telegram to the pool
                Pool.Free(telegram);
            }
        }

        public bool HandleMessage(Telegram message)
        {
            return false;
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private class MessageDispatcherPool : BasePool<Telegram>
        {
            public MessageDispatcherPool(int initialCapacity = 16, int maxCapacity = int.MaxValue)
                : base(initialCapacity, maxCapacity)
            {
            }

            protected override Telegram NewObject()
            {
                return new Telegram();
            }
        }
    }
}
