namespace Craiel.UnityEssentials.Runtime.AI.BTree
{
    public struct TaskId
    {
        public static readonly TaskId Invalid = new TaskId(0);
        public static readonly TaskId FirstValid = new TaskId(1);

        public readonly ushort Value;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public TaskId(ushort value)
        {
            this.Value = value;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static bool operator ==(TaskId value1, TaskId value2)
        {
            return value1.Value == value2.Value;
        }

        public static bool operator !=(TaskId value1, TaskId value2)
        {
            return value1.Value != value2.Value;
        }

        public bool Equals(TaskId other)
        {
            return this.Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is TaskId && this.Equals((TaskId)obj);
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        public TaskId GetNext()
        {
            return new TaskId((ushort)(this.Value + 1));
        }
    }
}
