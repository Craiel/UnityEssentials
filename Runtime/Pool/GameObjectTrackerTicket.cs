namespace Craiel.UnityEssentials.Runtime.Pool
{
    public struct GameObjectTrackerTicket
    {
        public static readonly GameObjectTrackerTicket Invalid = new GameObjectTrackerTicket(ushort.MaxValue);
        
        private static ushort nextId = 1;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public GameObjectTrackerTicket(ushort id)
        {
            this.Id = id;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public readonly ushort Id;

        public static bool operator ==(GameObjectTrackerTicket value1, GameObjectTrackerTicket value2)
        {
            return value1.Equals(value2);
        }

        public static bool operator !=(GameObjectTrackerTicket value1, GameObjectTrackerTicket value2)
        {
            return !(value1 == value2);
        }

        public override bool Equals(object obj)
        {
            var typed = (GameObjectTrackerTicket)obj;
            return typed.Id == this.Id;
        }

        public bool Equals(GameObjectTrackerTicket other)
        {
            return this.Id == other.Id;
        }

        public override int GetHashCode()
        {
            return this.Id;
        }

        public static GameObjectTrackerTicket Next()
        {
            return new GameObjectTrackerTicket(nextId++);
        }

        public override string ToString()
        {
            return string.Format("#GameObjectTrackerTicket:{0}", this.Id);
        }
    }
}