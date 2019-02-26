namespace Craiel.UnityEssentials.Runtime.TweenLite
{
    public struct TweenLiteTicket
    {
        public static readonly TweenLiteTicket Invalid = new TweenLiteTicket(0);

        private static uint nextId = 1;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public TweenLiteTicket(uint id)
        {
            this.Id = id;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public readonly uint Id;

        public static bool operator ==(TweenLiteTicket value1, TweenLiteTicket value2)
        {
            return value1.Equals(value2);
        }

        public static bool operator !=(TweenLiteTicket value1, TweenLiteTicket value2)
        {
            return !(value1 == value2);
        }

        public override bool Equals(object obj)
        {
            var typed = (TweenLiteTicket)obj;
            return typed.Id == this.Id;
        }

        public bool Equals(TweenLiteTicket other)
        {
            return this.Id == other.Id;
        }

        public override int GetHashCode()
        {
            return (int)this.Id;
        }

        public static TweenLiteTicket Next()
        {
            return new TweenLiteTicket(nextId++);
        }

        public override string ToString()
        {
            return string.Format("#Audio:{0}", this.Id);
        }
    }
}