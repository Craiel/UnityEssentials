namespace Craiel.UnityEssentials.Runtime.Extensions
{
    using UnityEngine;

    public static class BoundsExtensions
    {
        public static Bounds FromMinMax(Vector3 min, Vector3 max)
        {
            var result = new Bounds();
            result.SetMinMax(min, max);
            return result;
        }
    }
}
