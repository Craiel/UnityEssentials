namespace Craiel.UnityEssentials.Event
{
    using System;
    using System.Reflection;

    internal class BaseEventTargetCollection<T>
        where T : class
    {
        private const byte DefaultEventTargetSize = 100;

        private int nextFreeIndex;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public BaseEventTargetCollection()
        {
            this.Targets = new BaseEventSubscriptionTicket[0];

            // Do one increase to set the initial size
            this.IncreaseSize();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        // Fields are intentional for performance
        public BaseEventSubscriptionTicket[] Targets;

        public int Capacity;
        public int Occupied;

        public void Add(BaseEventSubscriptionTicket newTarget)
        {
            if (this.Occupied == this.Capacity)
            {
                this.IncreaseSize();
            }

#if DEBUG
            if (this.IsDuplicate(newTarget))
            {
                throw new InvalidOperationException("Target is already subscribed to event");
            }
#endif

            this.Targets[this.nextFreeIndex] = newTarget;
            this.Occupied++;

            this.FindFreeIndex();
        }

        public bool Remove(BaseEventSubscriptionTicket target)
        {
            if (this.Occupied == 0)
            {
                return false;
            }

            int checkCount = 0;
            for (var i = 0; i < this.Targets.Length; i++)
            {
                if (this.Targets[i] == null)
                {
                    continue;
                }

                if (this.Targets[i] == target)
                {
                    this.ClearSlot(i);
                    return true;
                }

                checkCount++;
                if (checkCount == this.Occupied)
                {
                    return false;
                }
            }

            return false;
        }

        public void Send<TSpecific>(TSpecific eventData)
            where TSpecific : T
        {
            int sentCount = 0;
            for (var i = 0; i < this.Targets.Length; i++)
            {
                if (this.Targets[i] == null)
                {
                    continue;
                }
                
                BaseEventSubscriptionTicket target = this.Targets[i];
                if (target.FilterDelegate != null)
                {
                    if (target.FilterDelegate(eventData))
                    {
                        ((BaseEventAggregate<T>.GameEventAction<TSpecific>) target.TargetDelegate).Invoke(eventData);
                    }
                }
                else
                {
                    ((BaseEventAggregate<T>.GameEventAction<TSpecific>)target.TargetDelegate).Invoke(eventData);
                }

                sentCount++;
                if (sentCount == this.Occupied)
                {
                    // Avoid loop if we already covered all targets
                    break;
                }
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void ClearSlot(int index)
        {
            this.Targets[index] = null;
            this.Occupied--;
            if (this.nextFreeIndex > index)
            {
                this.nextFreeIndex = index;
            }
        }

#if DEBUG
        private bool IsDuplicate(BaseEventSubscriptionTicket target)
        {
            if (this.Occupied == 0)
            {
                return false;
            }
            
            Type eventType = target.TargetDelegate.GetType();
            PropertyInfo methodProperty = eventType.GetProperty("Method");
            PropertyInfo targetProperty = eventType.GetProperty("Target");

            int checkCount = 0;
            for (var i = 0; i < this.Targets.Length; i++)
            {
                if (this.Targets[i] == null)
                {
                    continue;
                }

                // ReSharper disable once PossibleNullReferenceException
                var existingMethod = methodProperty.GetValue(this.Targets[i].TargetDelegate, null);

                // ReSharper disable once PossibleNullReferenceException
                var existingTargetObject = targetProperty.GetValue(this.Targets[i].TargetDelegate, null);

                var targetMethod = methodProperty.GetValue(target.TargetDelegate, null);
                var targetObject = targetProperty.GetValue(target.TargetDelegate, null);

                if (object.ReferenceEquals(existingMethod, targetMethod) && object.ReferenceEquals(existingTargetObject, targetObject))
                {
                    return true;
                }

                checkCount++;
                if (checkCount == this.Occupied)
                {
                    break;
                }
            }

            return false;
        }
#endif

        private void IncreaseSize()
        {
            Array.Resize(ref this.Targets, this.Targets.Length + DefaultEventTargetSize);
            this.Capacity = this.Targets.Length;

            this.nextFreeIndex = 0;
            this.FindFreeIndex();
        }

        private void FindFreeIndex()
        {
            for (var i = this.nextFreeIndex; i < this.Targets.Length; i++)
            {
                if (this.Targets[i] == null)
                {
                    this.nextFreeIndex = i;
                    break;
                }
            }
        }
    }
}
