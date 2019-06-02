namespace Craiel.UnityEssentials.Runtime.Noise
{
    using Enums;
    using UnityEngine;

    // Adapted for Unity from https://github.com/Auburns/FastNoise_CSharp
    public partial class NoiseProvider
    {
        private float fractalBounding;
        private float gain = 0.5f;
        private float lacunarity = 2.0f;
        private float frequency = 0.01f;
        private int octaves = 3;
        private int seed;

        private NoiseInterpolation interpolation = NoiseInterpolation.Quintic;
        private NoiseType noiseType = NoiseType.Value;
        
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public NoiseProvider()
        {
            this.CalculateFractalBounding();
            
            this.Reseed(NoiseConstants.DefaultSeed);
        }
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public int Seed
        {
            get { return this.seed; }
        }

        public float Gain
        {
	        get { return this.gain; }
	        set { this.gain = value; }
        }

        public float Lacunarity
        {
	        get { return this.lacunarity; }
	        set { this.lacunarity = value; }
        }

        public int Octaves
        {
	        get { return this.octaves; }
	        set { this.octaves = value; }
        }

        public NoiseType NoiseType
        {
	        get { return this.noiseType; }
	        set { this.noiseType = value; }
        }

        public void Reseed(int newSeed)
        {
            this.seed = newSeed;
        }

        private float GetCellular(Vector3 point)
        {
	        switch (this.cellularReturn)
	        {
		        case NoiseCellularReturn.CellValue:
		        case NoiseCellularReturn.NoiseLookup:
		        case NoiseCellularReturn.Distance:
		        {
			        return this.GetCellularSingle(point);
		        }
		        
		        default:
		        {
			        return this.GetCellular2Edge(point);
		        }
	        }
        }

        public float Get(Vector2 point)
        {
	        point.x *= this.frequency;
	        point.y *= this.frequency;
	        
	        switch (this.noiseType)
	        {
		        case NoiseType.WhiteNoise:
		        {
			        return this.GetWhiteNoise(point);
		        }

		        case NoiseType.Cubic:
		        {
			        return GetCubic(this.seed, point);
		        }

		        default:
		        {
			        EssentialsCore.Logger.Warn("Get Noise not supported for Vector2 and {0}", this.noiseType);
			        return 0;
		        }
	        }
        }
        
        public float Get(Vector3 point)
        {
	        point.x *= this.frequency;
	        point.y *= this.frequency;
	        point.z *= this.frequency;

	        switch (this.noiseType)
	        {
		        case NoiseType.Value:
		        {
			        return GetBaseValue(this.seed, point);
		        }

		        case NoiseType.Fractal:
		        {
			        return this.GetFractal(point);
		        }

		        case NoiseType.Perlin:
		        {
			        return this.GetPerlin(this.seed, point);
		        }

		        case NoiseType.PerlinFractal:
		        {
			        return this.GetFractalPerlin(point);
		        }

		        case NoiseType.Simplex:
		        {
			        return GetSimplex(this.seed, point);
		        }

		        case NoiseType.SimplexFractal:
		        {
			        return this.GetFractalSimplex(point);
		        }

		        case NoiseType.Cellular:
		        {
			        return this.GetCellular(point);
		        }

		        case NoiseType.CubicFractal:
		        {
			        return this.GetFractalCubic(point);
		        }
		        
		        case NoiseType.WhiteNoise:
		        {
			        return this.GetWhiteNoise(point);
		        }

		        default:
		        {
			        EssentialsCore.Logger.Warn("Get Noise not supported for Vector3 and {0}", this.noiseType);
			        return 0;
		        }
	        }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void CalculateFractalBounding()
        {
            float amp = this.gain;
            float ampFractal = 1;
            for (int i = 1; i < this.octaves; i++)
            {
                ampFractal += amp;
                amp *= this.gain;
            }

            this.fractalBounding = 1 / ampFractal;
        }
        
        private float GetBaseValue(int localSeed, Vector3 point)
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

                default:
                {
                    xs = point.x - x0;
                    ys = point.y - y0;
                    zs = point.z - z0;
                    break;
                }
            }

            float xf00 = NoiseConstants.Lerp(NoiseConstants.ValCoord3D(localSeed, x0, y0, z0), NoiseConstants.ValCoord3D(localSeed, x1, y0, z0), xs);
            float xf10 = NoiseConstants.Lerp(NoiseConstants.ValCoord3D(localSeed, x0, y1, z0), NoiseConstants.ValCoord3D(localSeed, x1, y1, z0), xs);
            float xf01 = NoiseConstants.Lerp(NoiseConstants.ValCoord3D(localSeed, x0, y0, z1), NoiseConstants.ValCoord3D(localSeed, x1, y0, z1), xs);
            float xf11 = NoiseConstants.Lerp(NoiseConstants.ValCoord3D(localSeed, x0, y1, z1), NoiseConstants.ValCoord3D(localSeed, x1, y1, z1), xs);

            float yf0 = NoiseConstants.Lerp(xf00, xf10, ys);
            float yf1 = NoiseConstants.Lerp(xf01, xf11, ys);

            return NoiseConstants.Lerp(yf0, yf1, zs);
        }
        
        private float GetWhiteNoise(Vector3 point)
        {
	        int xi = NoiseConstants.FloatCast2Int(point.x);
	        int yi = NoiseConstants.FloatCast2Int(point.y);
	        int zi = NoiseConstants.FloatCast2Int(point.z);

	        return NoiseConstants.ValCoord3D(this.seed, xi, yi, zi);
        }
    }
}