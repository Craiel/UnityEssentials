namespace Craiel.UnityEssentials.Runtime.EngineCore
{
    using System.Collections;
    using System.Collections.Generic;

    public class TicketProvider<T> : IEnumerable<T>
        where T : struct
    {
        private readonly HashSet<T> activeTickets;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public TicketProvider()
        {
            this.activeTickets = new HashSet<T>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public int Count
        {
            get { return this.activeTickets.Count; }
        }
        
        public bool IsUsed(T ticket)
        {
            return this.activeTickets.Contains(ticket);
        }
        
        public void Register(T ticket)
        {
            this.activeTickets.Add(ticket);
        }

        public bool Unregister(T ticket)
        {
            return this.activeTickets.Remove(ticket);
        }
        
        public IEnumerator<T> GetEnumerator()
        {
            return this.activeTickets.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.activeTickets.GetEnumerator();
        }
    }
}