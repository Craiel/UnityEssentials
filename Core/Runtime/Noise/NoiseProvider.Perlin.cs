namespace Craiel.UnityEssentials.Runtime.Noise
{
    using Enums;
    using UnityEngine;

    public partial class NoiseProvider
    {
        private float GetPerlin(int seed, Vector3 point)
        {
            int x0 = NoiseConstants.Floor(point.x);
            int y0 = NoiseConstants.Floor(point.y);
            int z0 = NoiseConstants.Floor(point.z);
            int x1 = x0 + 1;
            int y1 = y0 + 1;
            int z1 = z0 + 1;

            float xs, ys, zs;
            switch (this.interpolation)
            {
                default:
                {
                    xs = point.x - x0;
                    ys = point.y - y0;
                    zs = point.z - z0;
                    break;
                }

                case NoiseInterpolation.Hermite:
                {
                    xs = NoiseConstants.InterpolateHermite(point.x - x0);
                    ys = NoiseConstants.InterpolateHermite(point.y - y0);
                    zs = NoiseConstants.InterpolateHermite(point.z - z0);
                    break;
                }

                case NoiseInterpolation.Quintic:
                {
                    xs = NoiseConstants.InterpolateQuintic(point.x - x0);
                    ys = NoiseConstants.InterpolateQuintic(point.y - y0);
                    zs = NoiseConstants.InterpolateQuintic(point.z - z0);
                    break;
                }
            }

            float xd0 = point.x - x0;
            float yd0 = point.y - y0;
            float zd0 = point.z - z0;
            float xd1 = xd0 - 1;
            float yd1 = yd0 - 1;
            float zd1 = zd0 - 1;

            float xf00 = NoiseConstants.Lerp(NoiseConstants.GradCoord3D(seed, x0, y0, z0, xd0, yd0, zd0), NoiseConstants.GradCoord3D(seed, x1, y0, z0, xd1, yd0, zd0), xs);
            float xf10 = NoiseConstants.Lerp(NoiseConstants.GradCoord3D(seed, x0, y1, z0, xd0, yd1, zd0), NoiseConstants.GradCoord3D(seed, x1, y1, z0, xd1, yd1, zd0), xs);
            float xf01 = NoiseConstants.Lerp(NoiseConstants.GradCoord3D(seed, x0, y0, z1, xd0, yd0, zd1), NoiseConstants.GradCoord3D(seed, x1, y0, z1, xd1, yd0, zd1), xs);
            float xf11 = NoiseConstants.Lerp(NoiseConstants.GradCoord3D(seed, x0, y1, z1, xd0, yd1, zd1), NoiseConstants.GradCoord3D(seed, x1, y1, z1, xd1, yd1, zd1), xs);

            float yf0 = NoiseConstants.Lerp(xf00, xf10, ys);
            float yf1 = NoiseConstants.Lerp(xf01, xf11, ys);

            return NoiseConstants.Lerp(yf0, yf1, zs);
        }
        
        private float GetPerlinFractalFBM(Vector3 point)
        {
            int localSeed = this.seed;
            float sum = GetPerlin(localSeed, point);
            float amp = 1;

            for (int i = 1; i < this.octaves; i++)
            {
                point.x *= this.lacunarity;
                point.y *= this.lacunarity;
                point.z *= this.lacunarity;

                amp *= this.gain;
                sum += GetPerlin(++localSeed, point) * amp;
            }

            return sum * this.fractalBounding;
        }

        private float GetPerlinFractalBillow(Vector3 point)
        {
            int localSeec = this.seed;
            float sum = Mathf.Abs(GetPerlin(localSeec, point)) * 2 - 1;
            float amp = 1;

            for (int i = 1; i < this.octaves; i++)
            {
                point.x *= this.lacunarity;
                point.y *= this.lacunarity;
                point.z *= this.lacunarity;

                amp *= this.gain;
                sum += (Mathf.Abs(GetPerlin(++localSeec, point)) * 2 - 1) * amp;
            }

            return sum * this.fractalBounding;
        }

        private float GetPerlinFractalRigidMulti(Vector3 point)
        {
            int localSeed = this.seed;
            float sum = 1 - Mathf.Abs(GetPerlin(localSeed, point));
            float amp = 1;

            for (int i = 1; i < this.octaves; i++)
            {
                point.x *= this.lacunarity;
                point.y *= this.lacunarity;
                point.z *= this.lacunarity;

                amp *= this.gain;
                sum -= (1 - Mathf.Abs(GetPerlin(++localSeed, point))) * amp;
            }

            return sum;
        }
    }
}