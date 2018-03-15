namespace Craiel.UnityEssentials.Extensions
{
    using UnityEngine;

    public static class VectorExtensions
    {
        public static float[] ToArray(this Vector3 vector)
        {
            var result = new float[3];
            result[0] = vector.x;
            result[1] = vector.y;
            result[2] = vector.z;
            return result;
        }

        public static Vector3 Fill(float value)
        {
            return new Vector3(value, value, value);
        }
    }
}
