namespace Craiel.UnityEssentials.Runtime.Resource
{
    public static class ResourceExtensions
    {
        public static T LoadManaged<T>(this ResourceKey key)
            where T : UnityEngine.Object
        {
            using (var resource = ResourceProvider.Instance.AcquireOrLoadResource<T>(key))
            {
                if (resource == null)
                {
                    return default;
                }

                return resource.Data;
            }
        }
    }
}