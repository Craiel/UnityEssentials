namespace Craiel.UnityEssentials.Runtime.Pool
{
    using System;
    using Unity.Collections;
    using UnityEngine;
    using UnityEngine.Jobs;

    public class GameObjectTracker : IDisposable
    {
        private readonly GameObject[][] objectTrackers;
        private readonly Transform[][] transformTrackers;
        private readonly TransformAccessArray[] transformTrackerAccess;
        private readonly bool[] transformTrackerAccessValid;
        private readonly Renderer[][] rendererTrackers;
        private readonly NativeArray<float>[] aliveTimeTrackers;

        private readonly int[] trackerCount;

        private readonly ushort capacity;
        private readonly byte trackers;
        private readonly bool trackRenderers;
        
        private GameObjectTrackerEntry[] entries;

        private ushort nextFreeRegister;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public GameObjectTracker(byte trackerCount = 1, bool trackRenderers = false)
            : this(ushort.MaxValue - 1, trackerCount, trackRenderers)
        {
        }
        
        public GameObjectTracker(ushort capacity, byte trackerCount = 1, bool trackRenderers = false)
        {
            if (trackerCount == 0 || capacity == 0 || capacity == ushort.MaxValue)
            {
                throw new ArgumentException();
            }
            
            this.capacity = capacity;
            this.trackers = trackerCount;
            this.trackRenderers = trackRenderers;
            
            this.entries = new GameObjectTrackerEntry[this.capacity];
            
            this.objectTrackers = new GameObject[this.trackers][];
            this.transformTrackers = new Transform[this.trackers][];
            this.transformTrackerAccess = new TransformAccessArray[this.trackers];
            this.transformTrackerAccessValid = new bool[this.trackers];
            this.aliveTimeTrackers = new NativeArray<float>[this.trackers];

            if (trackRenderers)
            {
                this.rendererTrackers = new Renderer[this.trackers][];
            }
            
            this.trackerCount = new int[this.trackers];
            for (var i = 0; i < this.trackers; i++)
            {
                this.objectTrackers[i] = new GameObject[this.capacity];
                this.transformTrackers[i] = new Transform[this.capacity];
                this.aliveTimeTrackers[i] = new NativeArray<float>(this.capacity, Allocator.Persistent);

                if (trackRenderers)
                {
                    this.rendererTrackers[i] = new Renderer[this.capacity];
                }
            }
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public ushort Capacity
        {
            get { return this.capacity; }
        }

        public byte Trackers
        {
            get { return this.trackers; }
        }
        
        public int Entries { get; private set; }

        public int GetTrackerCount(byte tracker)
        {
            return this.trackerCount[tracker];
        }

        public GameObject Get(GameObjectTrackerTicket ticket)
        {
            GameObjectTrackerEntry entry = this.GetEntry(ticket);
            if (entry != null && entry.Object.TryGetTarget(out GameObject target))
            {
                return target;
            }

            return null;
        }

        public GameObjectTrackerTicket Get(int index)
        {
            if (this.entries[index] == null)
            {
                return GameObjectTrackerTicket.Invalid;
            }
            
            return this.entries[index].Ticket;
        }

        public GameObject[] GetObjects(byte tracker)
        {
            return this.objectTrackers[tracker];
        }

        public Transform[] GetTransforms(byte tracker)
        {
            return this.transformTrackers[tracker];
        }

        public TransformAccessArray GetTransformAccess(byte tracker)
        {
            if (!this.transformTrackerAccessValid[tracker])
            {
                if (this.transformTrackerAccess[tracker].isCreated)
                {
                    this.transformTrackerAccess[tracker].Dispose();
                }

                this.transformTrackerAccess[tracker] = new TransformAccessArray(this.transformTrackers[tracker]);
                this.transformTrackerAccessValid[tracker] = true;
            }
            
            return this.transformTrackerAccess[tracker];
        }

        public Renderer[] GetRenderers(byte tracker)
        {
            return this.trackRenderers ? this.rendererTrackers[tracker] : null;
        }

        public NativeArray<float> GetAliveTimes(byte tracker)
        {
            return this.aliveTimeTrackers[tracker];
        }

        public void Register(GameObject obj, out GameObjectTrackerTicket ticket)
        {
            if (this.nextFreeRegister == ushort.MaxValue)
            {
                throw new InvalidOperationException("Capacity exceeded");
            }
            
            ticket = new GameObjectTrackerTicket(this.nextFreeRegister);
            var entry = new GameObjectTrackerEntry(ticket, obj, this.Trackers);
            this.entries[ticket.Id] = entry;
            
            this.Entries++;
            
            this.FindNextFreeRegister();
        }

        public void Unregister(ref GameObjectTrackerTicket ticket)
        {
            GameObjectTrackerEntry entry = this.GetEntry(ticket);
            if (entry == null)
            {
                return;
            }

            this.UntrackAll(ticket);
            this.entries[ticket.Id] = null;

            this.Entries--;

            if (this.nextFreeRegister == ushort.MaxValue)
            {
                this.nextFreeRegister = ticket.Id;
            }
            
            ticket = GameObjectTrackerTicket.Invalid;
        }

        public void Track(GameObjectTrackerTicket ticket, byte tracker = 0)
        {
            GameObjectTrackerEntry entry = this.GetEntry(ticket);
            if (entry == null)
            {
                return;
            }

            this.DoTrack(entry, tracker);
        }

        public void TrackAll(GameObjectTrackerTicket ticket)
        {
            GameObjectTrackerEntry entry = this.GetEntry(ticket);
            if (entry == null)
            {
                return;
            }

            for (byte i = 0; i < this.trackers; i++)
            {
                this.DoTrack(entry, i);
            }
        }

        public void Untrack(GameObjectTrackerTicket ticket, byte tracker = 0)
        {
            GameObjectTrackerEntry entry = this.GetEntry(ticket);
            if (entry == null)
            {
                return;
            }
            
            this.DoUntrack(entry, tracker);
        }

        public void UntrackAll(GameObjectTrackerTicket ticket)
        {
            GameObjectTrackerEntry entry = this.GetEntry(ticket);
            if (entry == null)
            {
                return;
            }
            
            for (byte i = 0; i < this.trackers; i++)
            {
                this.DoUntrack(entry, i);
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                for (byte i = 0; i < this.trackers; i++)
                {
                    if (this.transformTrackerAccess[i].isCreated)
                    {
                        this.transformTrackerAccess[i].Dispose();
                    }
                    
                    this.aliveTimeTrackers[i].Dispose();
                }
            }
        }
        
        private void DoTrack(GameObjectTrackerEntry entry, byte tracker)
        {
            if (entry.TrackerIndex[tracker])
            {
                return;
            }
            
            if (!entry.Object.TryGetTarget(out GameObject target))
            {
                return;
            }

            ushort ticketId = entry.Ticket.Id;
            this.objectTrackers[tracker][ticketId] = target;
            this.transformTrackers[tracker][ticketId] = target.transform;
            this.transformTrackerAccessValid[tracker] = false;
            this.aliveTimeTrackers[tracker][ticketId] = entry.AliveTime;

            if (this.trackRenderers)
            {
                this.rendererTrackers[tracker][ticketId] = target.GetComponent<Renderer>();
            }

            entry.TrackerIndex[tracker] = true;
        }
        
        private void DoUntrack(GameObjectTrackerEntry entry, byte tracker)
        {
            if (!entry.TrackerIndex[tracker])
            {
                return;
            }
            
            ushort ticketId = entry.Ticket.Id;
            this.objectTrackers[tracker][ticketId] = null;
            this.transformTrackers[tracker][ticketId] = null;
            this.transformTrackerAccessValid[tracker] = false;
            this.aliveTimeTrackers[tracker][ticketId] = 0f;

            if (this.trackRenderers)
            {
                this.rendererTrackers[tracker][ticketId] = null;
            }

            entry.TrackerIndex[tracker] = false;
        }

        private GameObjectTrackerEntry GetEntry(GameObjectTrackerTicket ticket)
        {
            if (ticket == GameObjectTrackerTicket.Invalid)
            {
                throw new ArgumentException();
            }

            return this.entries[ticket.Id];
        }

        private void FindNextFreeRegister()
        {
            for (var n = 0; n < this.entries.Length; n++)
            {
                this.nextFreeRegister++;
                if (this.nextFreeRegister == this.entries.Length)
                {
                    // Loop around
                    this.nextFreeRegister = 0;
                }

                if (this.entries[this.nextFreeRegister] == null)
                {
                    return;
                }
            }

            // No free entry
            this.nextFreeRegister = ushort.MaxValue;
        }
    }
}