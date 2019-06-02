namespace Craiel.UnityEssentials.Runtime.Noise
{
    using UnityEngine;

    public partial class NoiseProvider
    {
        private const float SimplexF2 = 1.0f / 2.0f;
        private const float SimplexG2 = 1.0f / 4.0f;
        
        private static float GetSimplex(int seed, Vector2 point)
        {
            float t = (point.x + point.y) * SimplexF2;
            int i = NoiseConstants.Floor(point.x + t);
            int j = NoiseConstants.Floor(point.y + t);

            t = (i + j) * SimplexG2;
            float x0 = i - t;
            float y0 = j - t;

            x0 = point.x - x0;
            y0 = point.y - y0;

            int i1, j1;
            if (x0 > y0)
            {
                i1 = 1; j1 = 0;
            }
            else
            {
                i1 = 0; j1 = 1;
            }

            float x1 = x0 - i1 + SimplexG2;
            float y1 = y0 - j1 + SimplexG2;
            float x2 = x0 - 1 + SimplexF2;
            float y2 = y0 - 1 + SimplexF2;

            float n0, n1, n2;

            t = (float)0.5 - x0 * x0 - y0 * y0;
            if (t < 0)
            {
                n0 = 0;
            }
            else
            {
                t *= t;
                n0 = t * t * NoiseConstants.GradCoord2D(seed, i, j, x0, y0);
            }

            t = (float)0.5 - x1 * x1 - y1 * y1;
            if (t < 0)
            {
                n1 = 0;
            }
            else
            {
                t *= t;
                n1 = t * t * NoiseConstants.GradCoord2D(seed, i + i1, j + j1, x1, y1);
            }

            t = (float)0.5 - x2 * x2 - y2 * y2;
            if (t < 0)
            {
                n2 = 0;
            }
            else
            {
                t *= t;
                n2 = t * t * NoiseConstants.GradCoord2D(seed, i + 1, j + 1, x2, y2);
            }

            return 50 * (n0 + n1 + n2);
        }
        
        private float GetSimplexFractalFBM(Vector2 point)
        {
            int localSeed = this.seed;
            float sum = GetSimplex(localSeed, point);
            float amp = 1;

            for (int i = 1; i < this.octaves; i++)
            {
                point.x *= this.lacunarity;
                point.y *= this.lacunarity;

                amp *= this.gain;
                sum += GetSimplex(++localSeed, point) * amp;
            }

            return sum * this.fractalBounding;
        }

        private float GetSimplexFractalBillow(Vector2 point)
        {
            int localSeed = this.seed;
            float sum = Mathf.Abs(GetSimplex(localSeed, point)) * 2 - 1;
            float amp = 1;

            for (int i = 1; i < this.octaves; i++)
            {
                point.x *= this.lacunarity;
                point.y *= this.lacunarity;

                amp *= this.gain;
                sum += (Mathf.Abs(GetSimplex(++localSeed, point)) * 2 - 1) * amp;
            }

            return sum * this.fractalBounding;
        }

        private float GetSimplexFractalRigidMulti(Vector2 point)
        {
            int localSeed = this.seed;
            float sum = 1 - Mathf.Abs(GetSimplex(localSeed, point));
            float amp = 1;

            for (int i = 1; i < this.octaves; i++)
            {
                point.x *= this.lacunarity;
                point.y *= this.lacunarity;

                amp *= this.gain;
                sum -= (1 - Mathf.Abs(GetSimplex(++localSeed, point))) * amp;
            }

            return sum;
        }
    }
}