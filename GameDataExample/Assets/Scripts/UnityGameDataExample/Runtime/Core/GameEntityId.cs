namespace UnityGameDataExample.Runtime.Core
{
    using Enums;

    public struct GameEntityId
    {
        public static readonly GameEntityId Invalid = new GameEntityId(0, GameEntityType.Unknown);
        public static readonly GameEntityId Debug = new GameEntityId(ulong.MaxValue, GameEntityType.Debug);

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public GameEntityId(ulong value, GameEntityType type)
        {
            this.Value = value;
            this.Type = type;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public readonly ulong Value;

        public readonly GameEntityType Type;

        public static bool operator ==(GameEntityId value1, GameEntityId value2)
        {
            return value1.Value == value2.Value;
        }

        public static bool operator !=(GameEntityId value1, GameEntityId value2)
        {
            return value1.Value != value2.Value;
        }

        public bool Equals(GameEntityId other)
        {
            return this.Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is GameEntityId && this.Equals((GameEntityId)obj);
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        public override string ToString()
        {
            return string.Concat(this.Type, ':', this.Value);
        }
    }
}
