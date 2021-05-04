namespace Craiel.UnityEssentials.Runtime.Extensions
{
    using System;
    using UnityEngine;
    using Utils;

    public static class QuaternionCompression
    {
        private const float RotationCompressionPrecision = 32767f;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static long Compress(this Quaternion rotation)
        {
            var maxIndex = (byte)0;
            var maxValue = float.MinValue;
            var sign = 1f;

            float[] values = {rotation.x, rotation.y, rotation.z, rotation.w};

            // Determine the index of the largest (absolute value) element in the Quaternion.
            // We will transmit only the three smallest elements, and reconstruct the largest
            // element during decoding. 
            for (int i = 0; i < 4; i++)
            {
                var element = values[i];
                var abs = Math.Abs(values[i]);
                if (abs > maxValue)
                {
                    // We don't need to explicitly transmit the sign bit of the omitted element because you 
                    // can make the omitted element always positive by negating the entire quaternion if 
                    // the omitted element is negative (in quaternion space (x,y,z,w) and (-x,-y,-z,-w) 
                    // represent the same rotation.), but we need to keep track of the sign for use below.
                    sign = element < 0 ? -1 : 1;

                    // Keep track of the index of the largest element
                    maxIndex = (byte)i;
                    maxValue = abs;
                }
            }

            // If the maximum value is approximately 1f (such as Quaternion.identity [0,0,0,1]), then we can 
            // reduce storage even further due to the fact that all other fields must be 0f by definition, so 
            // we only need to send the index of the largest field.
            if (Math.Abs(maxValue - 1) < EssentialMathUtils.Epsilon)
            {
                // Again, don't need to transmit the sign since in quaternion space (x,y,z,w) and (-x,-y,-z,-w) 
                // represent the same rotation. We only need to send the index of the single element whose value
                // is 1f in order to recreate an equivalent rotation on the receiver.
                return maxIndex + 4;
            }

            short a;
            short b;
            short c;

            // We multiply the value of each element by RotationCompressionPrecision before converting to 16-bit integer 
            // in order to maintain precision. This is necessary since by definition each of the three smallest 
            // elements are less than 1.0, and the conversion to 16-bit integer would otherwise truncate everything 
            // to the right of the decimal place. This allows us to keep five decimal places.

            switch (maxIndex)
            {
                case 0:
                {
                    a = (short) (rotation.y * sign * RotationCompressionPrecision);
                    b = (short) (rotation.z * sign * RotationCompressionPrecision);
                    c = (short) (rotation.w * sign * RotationCompressionPrecision);
                    break;
                }

                case 1:
                {
                    a = (short) (rotation.x * sign * RotationCompressionPrecision);
                    b = (short) (rotation.z * sign * RotationCompressionPrecision);
                    c = (short) (rotation.w * sign * RotationCompressionPrecision);
                    break;
                }

                case 2:
                {
                    a = (short) (rotation.x * sign * RotationCompressionPrecision);
                    b = (short) (rotation.y * sign * RotationCompressionPrecision);
                    c = (short) (rotation.w * sign * RotationCompressionPrecision);
                    break;
                }

                default:
                {
                    a = (short) (rotation.x * sign * RotationCompressionPrecision);
                    b = (short) (rotation.y * sign * RotationCompressionPrecision);
                    c = (short) (rotation.z * sign * RotationCompressionPrecision);
                    break;
                }
            }

            return ((long)c << 48) + ((long)b << 32) + ((long)a << 16) + maxIndex;
        }
        
        public static Quaternion Decompress(long compressed)
        {
            byte maxIndex = (byte)(compressed & 0xff);
            short sourceA = (short)((compressed >> 16) & 0xffff);
            short sourceB = (short)((compressed >> 32) & 0xffff);
            short sourceC = (short)((compressed >> 48) & 0xffff);

            // Values between 4 and 7 indicate that only the index of the single field whose value is 1f was
            // sent, and (maxIndex - 4) is the correct index for that field.
            if (maxIndex >= 4 && maxIndex <= 7)
            {
                float x = maxIndex == 4 ? 1f : 0f;
                float y = maxIndex == 5 ? 1f : 0f;
                float z = maxIndex == 6 ? 1f : 0f;
                float w = maxIndex == 7 ? 1f : 0f;

                return new Quaternion(x, y, z, w);
            }

            // Read the other three fields and derive the value of the omitted field
            float a = sourceA / RotationCompressionPrecision;
            float b = sourceB / RotationCompressionPrecision;
            float c = sourceC / RotationCompressionPrecision;
            float d = (float)Math.Sqrt(1f - (a * a + b * b + c * c));

            if (maxIndex == 0)
            {
                return new Quaternion(d, a, b, c);
            }

            if (maxIndex == 1)
            {
                return new Quaternion(a, d, b, c);
            }

            if (maxIndex == 2)
            {
                return new Quaternion(a, b, d, c);
            }

            return new Quaternion(a, b, c, d);
        }
    }
}