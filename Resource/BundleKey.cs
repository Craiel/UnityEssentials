﻿namespace Assets.Scripts.Craiel.Essentials.Resource
{
    public struct BundleKey
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public BundleKey(int bundleVersion, string bundle)
            : this()
        {
            this.BundleVersion = bundleVersion;
            this.Bundle = bundle;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static BundleKey Invalid = new BundleKey(-1, null);
        
        public int BundleVersion { get; set; }
        
        public string Bundle { get; set; }
    
        public static bool operator ==(BundleKey rhs, BundleKey lhs)
        {
            return rhs.BundleVersion == lhs.BundleVersion 
                && rhs.Bundle == lhs.Bundle;
        }

        public static bool operator !=(BundleKey rhs, BundleKey lhs)
        {
            return !(rhs == lhs);
        }

        public override int GetHashCode()
        {
            return HashUtils.GetSimpleCombinedHashCode(this.BundleVersion, this.Bundle);
        }

        public override bool Equals(object other)
        {
            if (other == null || other.GetType() != typeof(BundleKey))
            {
                return false;
            }

            BundleKey typed = (BundleKey)other;
            return this == typed;
        }

        public override string ToString()
        {
            return string.Format("{0}@{1}", this.Bundle, this.BundleVersion);
        }
    }
}
