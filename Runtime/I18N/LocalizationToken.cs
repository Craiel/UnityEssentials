namespace Craiel.UnityEssentials.Runtime.I18N
{
    using System.Diagnostics;

    [DebuggerDisplay("{" + nameof(Key) + "}")]
    public struct LocalizationToken
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        internal LocalizationToken(string key)
        {
            this.Key = key;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public readonly string Key;

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            return ((LocalizationToken) obj).Equals(this);
        }

        public bool Equals(LocalizationToken other)
        {
            return string.Equals(this.Key, other.Key);
        }

        public override int GetHashCode()
        {
            return (this.Key != null ? this.Key.GetHashCode() : 0);
        }
    }
}