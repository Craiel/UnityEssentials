namespace Craiel.UnityEssentials.Runtime.EngineCore
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class TicketProvider<T, TD> : IEnumerable<TD>
        where T : struct
        where TD : ITicketData
    {
        public delegate bool ManagedTicketCheckDelegate(T ticket);
        public delegate void ManagedTicketFinishedDelegate(ref T ticket);
        
        private readonly IDictionary<T, TD> activeTickets;
        private readonly IList<TD> dataEntries;
        
        private readonly IList<T> managedTickets;
        private readonly IList<T> ticketTempList;

        private ManagedTicketCheckDelegate managedTicketFinishCheck;
        private ManagedTicketFinishedDelegate managedTicketFinished;

        private bool updateDataEntries;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public TicketProvider(bool updateDataEntries = false)
        {
            this.activeTickets = new Dictionary<T, TD>();
            this.dataEntries = new List<TD>();
            this.managedTickets = new List<T>();
            this.ticketTempList = new List<T>();

            this.updateDataEntries = updateDataEntries;
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
            if (this.updateDataEntries)
            {
                for (var i = 0; i < this.dataEntries.Count; i++)
                {
                    this.dataEntries[i].Update();
                }
            }

            this.UpdateManagedTickets();
        }
        
        public void Manage(T ticket)
        {
            if (!this.CanManageTickets)
            {
                throw new InvalidOperationException("Ticket Management is not configured!");
            }
            
#if DEBUG
            //EssentialsCore.Logger.Info("TICKET_MANAGE: {0}", ticket);
#endif
            
            this.managedTickets.Add(ticket);
        }
        
        public void Register(T ticket, TD data)
        {
#if DEBUG
            //EssentialsCore.Logger.Info("TICKET_REGISTER: {0}", ticket);
#endif
            
            this.activeTickets.Add(ticket, data);
            this.dataEntries.Add(data);
        }

        public bool Unregister(T ticket)
        {
            if (!this.TryGet(ticket, out TD ticketData))
            {
                return false;
            }
            
#if DEBUG
            //EssentialsCore.Logger.Info("TICKET_UNREGISTER: {0}", ticket);
#endif
            
            this.dataEntries.Remove(ticketData);
            this.managedTickets.Remove(ticket);
            return this.activeTickets.Remove(ticket);
        }

        public void EnableManagedTickets(ManagedTicketCheckDelegate finishedCheck, ManagedTicketFinishedDelegate onFinished)
        {
            this.managedTicketFinishCheck = finishedCheck;
            this.managedTicketFinished = onFinished;
            
            this.CanManageTickets = true;
        }
        
        public IEnumerator<TD> GetEnumerator()
        {
            return this.dataEntries.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.dataEntries.GetEnumerator();
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
                this.Unregister(this.ticketTempList[i]);
            }

            this.ticketTempList.Clear();
        }
    }
}