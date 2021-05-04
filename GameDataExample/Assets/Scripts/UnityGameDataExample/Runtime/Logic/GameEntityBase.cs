namespace UnityGameDataExample.Runtime.Logic
{
    using Core;

    public abstract class GameEntityBase
    {
        private readonly GameEntityId id;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        protected GameEntityBase(GameEntityId id)
        {
            this.id = id;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public GameEntityId Id
        {
            get { return this.id; }
        }

        public bool IsDead { get; protected set; }

        public override bool Equals(object obj)
        {
            return obj is GameEntityBase && this.Equals((GameEntityBase)obj);
        }

        public override int GetHashCode()
        {
            return this.id.GetHashCode();
        }

        public virtual void TickEntity()
        {
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected bool Equals(GameEntityBase other)
        {
            return this.Id.Equals(other.Id);
        }
    }
}
