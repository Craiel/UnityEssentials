namespace Craiel.UnityEssentials.Runtime.EngineCore
{
    using System;
    using System.Collections.Generic;

    public class TicketProvider<T, TD>
        where T : struct
    {
        public delegate bool ManagedTicketCheckDelegate(T ticket);
        public delegate void ManagedTicketFinishedDelegate(ref T ticket);
        
        private readonly IDictionary<T, TD> activeTickets;
        
        private readonly IList<T> managedTickets;
        private readonly IList<T> ticketTempList;

        private ManagedTicketCheckDelegate managedTicketFinishCheck;
        private ManagedTicketFinishedDelegate managedTicketFinished;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public TicketProvider()
        {
            this.activeTickets = new Dictionary<T, TD>();
            this.managedTickets = new List<T>();
            this.ticketTempList = new List<T>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public int Count
        {
            get { return this.activeTickets.Count; }
        }
        
        public bool CanManageTickets { get; private set; }
        
        public bool TryGet(T ticket, out TD data)
        {
            return this.activeTickets.TryGetValue(ticket, out data);
        }

        public void Update()
        {
            this.UpdateManagedTickets();
        }
        
        public void Manage(T ticket)
        {
            if (!this.CanManageTickets)
            {
                throw new InvalidOperationException("Ticket Management is not configured!");
            }
            
            this.managedTickets.Add(ticket);
        }
        
        public void Register(T ticket, TD data)
        {
            this.activeTickets.Add(ticket, data);
        }

        public bool Unregister(T ticket)
        {
            return this.activeTickets.Remove(ticket);
        }

        public void EnableManagedTickets(ManagedTicketCheckDelegate finishedCheck, ManagedTicketFinishedDelegate onFinished)
        {
            this.managedTicketFinishCheck = finishedCheck;
            this.managedTicketFinished = onFinished;
            
            this.CanManageTickets = true;
        }
        
        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void UpdateManagedTickets()
        {
            this.ticketTempList.Clear();
            for (var i = 0; i < this.managedTickets.Count; i++)
            {
                T ticket = this.managedTickets[i];
                if (this.managedTicketFinishCheck(ticket))
                {
                    this.ticketTempList.Add(ticket);
                    this.managedTicketFinished(ref ticket);
                }
            }

            for (var i = 0; i < this.ticketTempList.Count; i++)
            {
                this.managedTickets.Remove(this.ticketTempList[i]);
            }

            this.ticketTempList.Clear();
        }
    }
}