namespace Craiel.UnityEssentials.Runtime.Noise
{
	using System;
	using Enums;
	using UnityEngine;

	public partial class NoiseProvider
	{
		private NoiseCellularReturn cellularReturn = NoiseCellularReturn.CellValue;
		private NoiseCellularDistance cellularDistance = NoiseCellularDistance.Euclidean;
		
		private float cellularJitter = 0.45f;
		private int cellularDistanceIndex0 = 0;
		private int cellularDistanceIndex1 = 1;
		private NoiseProvider cellularLookup;

		// -------------------------------------------------------------------
		// Public
		// -------------------------------------------------------------------
		public NoiseCellularDistance CellularDistance
		{
			get { return this.cellularDistance; }
			set { this.cellularDistance = value; }
		}

		public NoiseCellularReturn CellularReturn
		{
			get { return this.cellularReturn; }
			set { this.cellularReturn = value; }
		}
		
		public float CellularJitter
		{
			get { return this.cellularJitter; }
			set { this.cellularJitter = value; }
		}

		public NoiseProvider CellularLookup
		{
			get { return this.cellularLookup; }
			set { this.cellularLookup = value; }
		}
		
		// -------------------------------------------------------------------
		// Private
		// -------------------------------------------------------------------
		private float GetCellularSingle(Vector2 point)
		{
			int xr = NoiseConstants.Round(point.x);
			int yr = NoiseConstants.Round(point.y);

			float distance = 999999;
			int xc = 0, yc = 0;

			switch (this.cellularDistance)
			{
				default:
				{
					for (int xi = xr - 1; xi <= xr + 1; xi++)
					{
						for (int yi = yr - 1; yi <= yr + 1; yi++)
						{
							Vector2 vec = NoiseConstants.Cell2D[NoiseConstants.Hash2D(this.seed, xi, yi) & 255];
							float vecX = xi - point.x + vec.x * this.cellularJitter;
							float vecY = yi - point.y + vec.y * this.cellularJitter;

							float newDistance = vecX * vecX + vecY * vecY;

							if (newDistance < distance)
							{
								distance = newDistance;
								xc = xi;
								yc = yi;
							}
						}
					}

					break;
				}

				case NoiseCellularDistance.Manhattan:
				{
					for (int xi = xr - 1; xi <= xr + 1; xi++)
					{
						for (int yi = yr - 1; yi <= yr + 1; yi++)
						{
							Vector2 vec = NoiseConstants.Cell2D[NoiseConstants.Hash2D(this.seed, xi, yi) & 255];

							float vecX = xi - point.x + vec.x * this.cellularJitter;
							float vecY = yi - point.y + vec.y * this.cellularJitter;

							float newDistance = (Math.Abs(vecX) + Math.Abs(vecY));

							if (newDistance < distance)
							{
								distance = newDistance;
								xc = xi;
								yc = yi;
							}
						}
					}

					break;
				}

				case NoiseCellularDistance.Natural:
				{
					for (int xi = xr - 1; xi <= xr + 1; xi++)
					{
						for (int yi = yr - 1; yi <= yr + 1; yi++)
						{
							Vector2 vec = NoiseConstants.Cell2D[NoiseConstants.Hash2D(this.seed, xi, yi) & 255];

							float vecX = xi - point.x + vec.x * this.cellularJitter;
							float vecY = yi - point.y + vec.y * this.cellularJitter;

							float newDistance = (Math.Abs(vecX) + Math.Abs(vecY)) + (vecX * vecX + vecY * vecY);

							if (newDistance < distance)
							{
								distance = newDistance;
								xc = xi;
								yc = yi;
							}
						}
					}

					break;
				}
			}

			switch (this.cellularReturn)
			{
				case NoiseCellularReturn.CellValue:
				{
					return NoiseConstants.ValCoord2D(this.seed, xc, yc);
				}

				case NoiseCellularReturn.NoiseLookup:
				{
					Vector2 vec = NoiseConstants.Cell2D[NoiseConstants.Hash2D(this.seed, xc, yc) & 255];
					return this.cellularLookup.Get(new Vector2(xc + vec.x * this.cellularJitter, yc + vec.y * this.cellularJitter));
				}

				case NoiseCellularReturn.Distance:
				{
					return distance;
				}
				default:
				{
					return 0;
				}
			}
		}

		private float GetCellular2Edge(Vector2 point)
		{
			int xr = NoiseConstants.Round(point.x);
			int yr = NoiseConstants.Round(point.y);

			float[] distance = {999999, 999999, 999999, 999999};

			switch (this.cellularDistance)
			{
				default:
					for (int xi = xr - 1; xi <= xr + 1; xi++)
					{
						for (int yi = yr - 1; yi <= yr + 1; yi++)
						{
							Vector2 vec = NoiseConstants.Cell2D[NoiseConstants.Hash2D(this.seed, xi, yi) & 255];

							float vecX = xi - point.x + vec.x * this.cellularJitter;
							float vecY = yi - point.y + vec.y * this.cellularJitter;

							float newDistance = vecX * vecX + vecY * vecY;

							for (int i = this.cellularDistanceIndex1; i > 0; i--)
							{
								distance[i] = Math.Max(Math.Min(distance[i], newDistance), distance[i - 1]);
							}
							
							distance[0] = Math.Min(distance[0], newDistance);
						}
					}

					break;
				case NoiseCellularDistance.Manhattan:
					for (int xi = xr - 1; xi <= xr + 1; xi++)
					{
						for (int yi = yr - 1; yi <= yr + 1; yi++)
						{
							Vector2 vec = NoiseConstants.Cell2D[NoiseConstants.Hash2D(this.seed, xi, yi) & 255];

							float vecX = xi - point.x + vec.x * this.cellularJitter;
							float vecY = yi - point.y + vec.y * this.cellularJitter;

							float newDistance = Math.Abs(vecX) + Math.Abs(vecY);

							for (int i = this.cellularDistanceIndex1; i > 0; i--)
							{
								distance[i] = Math.Max(Math.Min(distance[i], newDistance), distance[i - 1]);
							}
							
							distance[0] = Math.Min(distance[0], newDistance);
						}
					}

					break;
				case NoiseCellularDistance.Natural:
					for (int xi = xr - 1; xi <= xr + 1; xi++)
					{
						for (int yi = yr - 1; yi <= yr + 1; yi++)
						{
							Vector2 vec = NoiseConstants.Cell2D[NoiseConstants.Hash2D(this.seed, xi, yi) & 255];

							float vecX = xi - point.x + vec.x * this.cellularJitter;
							float vecY = yi - point.y + vec.y * this.cellularJitter;

							float newDistance = (Math.Abs(vecX) + Math.Abs(vecY)) + (vecX * vecX + vecY * vecY);

							for (int i = this.cellularDistanceIndex1; i > 0; i--)
							{
								distance[i] = Math.Max(Math.Min(distance[i], newDistance), distance[i - 1]);
							}
							
							distance[0] = Math.Min(distance[0], newDistance);
						}
					}

					break;
			}

			switch (this.cellularReturn)
			{
				case NoiseCellularReturn.Distance2:
				{
					return distance[this.cellularDistanceIndex1];
				}

				case NoiseCellularReturn.Distance2Add:
				{
					return distance[this.cellularDistanceIndex1] + distance[this.cellularDistanceIndex0];
				}

				case NoiseCellularReturn.Distance2Sub:
				{
					return distance[this.cellularDistanceIndex1] - distance[this.cellularDistanceIndex0];
				}

				case NoiseCellularReturn.Distance2Mul:
				{
					return distance[this.cellularDistanceIndex1] * distance[this.cellularDistanceIndex0];
				}

				case NoiseCellularReturn.Distance2Div:
				{
					return distance[this.cellularDistanceIndex0] / distance[this.cellularDistanceIndex1];
				}

				default:
				{
					return 0;
				}
			}
		}
	}
}