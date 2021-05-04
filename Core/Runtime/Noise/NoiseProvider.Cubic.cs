namespace Craiel.UnityEssentials.Runtime.Noise
{
    using UnityEngine;

    public partial class NoiseProvider
    {
        private const float Cubic2DBounding = 1 / (float)(1.5 * 1.5);
        
        private static float GetCubic(int targetSeed, Vector2 point)
        {
            int x1 = NoiseConstants.Floor(point.x);
            int y1 = NoiseConstants.Floor(point.y);

            int x0 = x1 - 1;
            int y0 = y1 - 1;
            int x2 = x1 + 1;
            int y2 = y1 + 1;
            int x3 = x1 + 2;
            int y3 = y1 + 2;

            float xs = point.x - x1;
            float ys = point.y - y1;

            return NoiseConstants.CubicLerp(
                       NoiseConstants.CubicLerp(NoiseConstants.ValCoord2D(targetSeed, x0, y0), NoiseConstants.ValCoord2D(targetSeed, x1, y0), NoiseConstants.ValCoord2D(targetSeed, x2, y0), NoiseConstants.ValCoord2D(targetSeed, x3, y0),xs),
                       NoiseConstants.CubicLerp(NoiseConstants.ValCoord2D(targetSeed, x0, y1), NoiseConstants.ValCoord2D(targetSeed, x1, y1), NoiseConstants.ValCoord2D(targetSeed, x2, y1), NoiseConstants.ValCoord2D(targetSeed, x3, y1),xs),
                       NoiseConstants.CubicLerp(NoiseConstants.ValCoord2D(targetSeed, x0, y2), NoiseConstants.ValCoord2D(targetSeed, x1, y2), NoiseConstants.ValCoord2D(targetSeed, x2, y2), NoiseConstants.ValCoord2D(targetSeed, x3, y2),xs),
                       NoiseConstants.CubicLerp(NoiseConstants.ValCoord2D(targetSeed, x0, y3), NoiseConstants.ValCoord2D(targetSeed, x1, y3), NoiseConstants.ValCoord2D(targetSeed, x2, y3), NoiseConstants.ValCoord2D(targetSeed, x3, y3),xs),
                       ys) * Cubic2DBounding;
        }
        
        private float GetCubicFractalFBM(Vector2 point)
        {
            int localSeec = this.seed;
            float sum = GetCubic(localSeec, point);
            float amp = 1;
            int i = 0;

            while (++i < this.octaves)
            {
                point.x *= this.lacunarity;
                point.y *= this.lacunarity;

                amp *= this.gain;
                sum += GetCubic(++localSeec, point) * amp;
            }

            return sum * this.fractalBounding;
        }

        private float GetCubicFractalBillow(Vector2 point)
        {
            int localSeed = this.seed;
            float sum = Mathf.Abs(GetCubic(localSeed, point)) * 2 - 1;
            float amp = 1;
            int i = 0;

            while (++i < this.octaves)
            {
                point.x *= this.lacunarity;
                point.y *= this.lacunarity;

                amp *= this.gain;
                sum += (Mathf.Abs(GetCubic(++localSeed, point)) * 2 - 1) * amp;
            }

            return sum * this.fractalBounding;
        }

        private float GetCubicFractalRigidMulti(Vector2 point)
        {
            int localSeed = this.seed;
            float sum = 1 - Mathf.Abs(GetCubic(localSeed, point));
            float amp = 1;
            int i = 0;

            while (++i < this.octaves)
            {
                point.x *= this.lacunarity;
                point.y *= this.lacunarity;

                amp *= this.gain;
                sum -= (1 - Mathf.Abs(GetCubic(++localSeed, point))) * amp;
            }

            return sum;
        }
    }
}