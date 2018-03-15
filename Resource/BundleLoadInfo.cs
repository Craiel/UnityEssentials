namespace Craiel.UnityEssentials.Resource
{
    using Enums;

    public struct BundleLoadInfo
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public BundleLoadInfo(BundleKey key, BundleLoadFlags flags)
            : this()
        {
            this.Key = key;
            this.Flags = flags;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public BundleKey Key { get; private set; }

        public BundleLoadFlags Flags { get; private set; }
    }
}
