namespace Craiel.UnityEssentials.Runtime.TweenLite
{
	using System;
	using Enums;
	using UnityEngine;
	using Utils;

	public partial class TweenLiteSystem
	{
		// -------------------------------------------------------------------
		// Public
		// -------------------------------------------------------------------
		public static float ApplyEasing(TweenLiteEasingMode mode, float delta, float min, float max, float duration)
		{
			switch (mode)
			{
				case TweenLiteEasingMode.Swing:
				{
					return -max * (delta /= duration) * (delta - 2f) + min;
				}

				case TweenLiteEasingMode.InQuad:
				{
					return max * (delta /= duration) * delta + min;
				}

				case TweenLiteEasingMode.OutQuad:
				{
					return -max * (delta /= duration) * (delta - 2) + min;
				}

				case TweenLiteEasingMode.InOutQuad:
				{
					if ((delta /= duration / 2) < 1)
					{
						return max / 2 * delta * delta + min;
					}
					
					return -max / 2 * ((--delta) * (delta - 2) - 1) + min;
				}

				case TweenLiteEasingMode.InCubic:
				{
					return max * (delta /= duration) * delta * delta + min;
				}

				case TweenLiteEasingMode.OutCubic:
				{
					return max * ((delta = delta / duration - 1) * delta * delta + 1) + min;
				}

				case TweenLiteEasingMode.InOutCubic:
				{
					if ((delta /= duration / 2) < 1)
					{
						return max / 2 * delta * delta * delta + min;
					}

					return max / 2 * ((delta -= 2) * delta * delta + 2) + min;
				}

				case TweenLiteEasingMode.InQuart:
				{
					return max * (delta /= duration) * delta * delta * delta + min;
				}

				case TweenLiteEasingMode.OutQuart:
				{
					return -max * ((delta = delta / duration - 1) * delta * delta * delta - 1) + min;
				}

				case TweenLiteEasingMode.InOutQuart:
				{
					if ((delta /= duration / 2) < 1)
					{
						return max / 2 * delta * delta * delta * delta + min;
					}
					
					return -max / 2 * ((delta -= 2) * delta * delta * delta - 2) + min;
				}
				
				case TweenLiteEasingMode.InQuint:
				{
					return max * (delta /= duration) * delta * delta * delta * delta + min;
				}
				
				case TweenLiteEasingMode.OutQuint:
				{
					return max * ((delta = delta / duration - 1) * delta * delta * delta * delta + 1) + min;
				}
				
				case TweenLiteEasingMode.InOutQuint:
				{
					if ((delta /= duration / 2) < 1)
					{
						return max / 2 * delta * delta * delta * delta * delta + min;
					}
					
					return max / 2 * ((delta -= 2) * delta * delta * delta * delta + 2) + min;
				}
				
				case TweenLiteEasingMode.InSine:
				{
					return -max * Mathf.Cos(delta / duration * (Mathf.PI / 2)) + max + min;
				}
				
				case TweenLiteEasingMode.OutSine:
				{
					return max * Mathf.Sin(delta / duration * (Mathf.PI / 2)) + min;
				}
				
				case TweenLiteEasingMode.InOutSine:
				{
					return -max / 2 * (Mathf.Cos(Mathf.PI * delta / duration) - 1) + min;
				}
				
				case TweenLiteEasingMode.InExpo:
				{
					return Math.Abs(delta) < EssentialMathUtils.Epsilon ? min : max * Mathf.Pow(2, 10 * (delta / duration - 1)) + min;
				}
				
				case TweenLiteEasingMode.OutExpo:
				{
					return Math.Abs(delta - duration) < EssentialMathUtils.Epsilon ? min + max : max * (-Mathf.Pow(2, -10 * delta / duration) + 1) + min;
				}
				
				case TweenLiteEasingMode.InOutExpo:
				{
					if (Math.Abs(delta) < EssentialMathUtils.Epsilon)
					{
						return min;
					}

					if (Math.Abs(delta - duration) < EssentialMathUtils.Epsilon)
					{
						return min + max;
					}

					if ((delta /= duration / 2) < 1)
					{
						return max / 2 * Mathf.Pow(2, 10 * (delta - 1)) + min;
					}
					
					return max / 2 * (-Mathf.Pow(2, -10 * --delta) + 2) + min;
				}
				
				case TweenLiteEasingMode.InCirc:
				{
					return -max * (Mathf.Sqrt(1 - (delta /= duration) * delta) - 1) + min;
				}
				
				case TweenLiteEasingMode.OutCirc:
				{
					return max * Mathf.Sqrt(1 - (delta = delta / duration - 1) * delta) + min;
				}
				
				case TweenLiteEasingMode.InOutCirc:
				{
					if ((delta /= duration / 2) < 1)
					{
						return -max / 2 * (Mathf.Sqrt(1 - delta * delta) - 1) + min;
					}
					
					return max / 2 * (Mathf.Sqrt(1 - (delta -= 2) * delta) + 1) + min;
				}
				
				case TweenLiteEasingMode.InBack:
				{
					float s = 1.70158f;
					return max * (delta /= duration) * delta * ((s + 1f) * delta - s) + min;
				}
				
				case TweenLiteEasingMode.OutBack:
				{
					float s = 1.70158f;
					return max * ((delta = delta / duration - 1f) * delta * ((s + 1f) * delta + s) + 1f) + min;
				}
				
				case TweenLiteEasingMode.InOutBack:
				{
					float s = 1.70158f;
					if ((delta /= duration / 2f) < 1f)
					{
						return max / 2f * (delta * delta * (((s *= (1.525f)) + 1f) * delta - s)) + min;
					}
					
					return max / 2f * ((delta -= 2f) * delta * (((s *= (1.525f)) + 1f) * delta + s) + 2f) + min;
				}
				
				case TweenLiteEasingMode.InBounce:
				{
					return max - ApplyEasing(TweenLiteEasingMode.OutBounce, duration - delta, 0f, max, duration) + min;
				}
				
				case TweenLiteEasingMode.OutBounce:
				{
					if ((delta /= duration) < (1f / 2.75f))
					{
						return max * (7.5625f * delta * delta) + min;
					}
					
					if (delta < (2f / 2.75f))
					{
						return max * (7.5625f * (delta -= (1.5f / 2.75f)) * delta + .75f) + min;
					}
					
					if (delta < (2.5f / 2.75f))
					{
						return max * (7.5625f * (delta -= (2.25f / 2.75f)) * delta + .9375f) + min;
					}
					
					return max * (7.5625f * (delta -= (2.625f / 2.75f)) * delta + .984375f) + min;
				}
				
				case TweenLiteEasingMode.InOutBounce:
				{
					if (delta < duration / 2f)
					{
						return ApplyEasing(TweenLiteEasingMode.InBounce, delta * 2f, 0f, max, duration) * .5f + min;
					}
					
					return ApplyEasing(TweenLiteEasingMode.OutBounce, delta * 2f - duration, 0f, max, duration) * .5f + max * .5f + min;
				}
				
				case TweenLiteEasingMode.InElastic:
				{
					float s;
					float p = 0f;
					float a = max;
					if (Math.Abs(delta) < EssentialMathUtils.Epsilon)
					{
						return min;
					}

					if (Math.Abs((delta /= duration) - 1f) < EssentialMathUtils.Epsilon)
					{
						return min + max;
					}

					if (Math.Abs(p) < EssentialMathUtils.Epsilon)
					{
						p = duration * .3f;
					}
					
					if (a < Mathf.Abs(max))
					{
						a = max;
						s = p / 4f;
					}
					else
					{
						s = p / (2f * Mathf.PI) * Mathf.Asin(max / a);
					}

					if (float.IsNaN(s))
					{
						s = 0f;
					}
					
					return -(a * Mathf.Pow(2f, 10f * (delta -= 1f)) * Mathf.Sin((delta * duration - s) * (2f * Mathf.PI) / p)) + min;
				}
				
				case TweenLiteEasingMode.OutElastic:
				{
					float s;
					float p = 0f;
					float a = max;
					if (Math.Abs(delta) < EssentialMathUtils.Epsilon)
					{
						return min;
					}

					if (Math.Abs((delta /= duration) - 1f) < EssentialMathUtils.Epsilon)
					{
						return min + max;
					}

					if (Math.Abs(p) < EssentialMathUtils.Epsilon)
					{
						p = duration * .3f;
					}
					
					if (a < Mathf.Abs(max))
					{
						a = max;
						s = p / 4f;
					}
					else
					{
						s = p / (2f * Mathf.PI) * Mathf.Asin(max / a);
					}

					if (float.IsNaN(s))
					{
						s = 0f;
					}
					
					return a * Mathf.Pow(2f, -10f * delta) * Mathf.Sin((delta * duration - s) * (2f * Mathf.PI) / p) + max + min;
				}
				
				case TweenLiteEasingMode.InOutElastic:
				{
					float s;
					float p = 0f;
					float a = max;
					if (Math.Abs(delta) < EssentialMathUtils.Epsilon)
					{
						return min;
					}

					if (Math.Abs((delta /= duration / 2f) - 2f) < EssentialMathUtils.Epsilon)
					{
						return min + max;
					}

					if (Math.Abs(p) < EssentialMathUtils.Epsilon)
					{
						p = duration * (.3f * 1.5f);
					}
					
					if (a < Mathf.Abs(max))
					{
						a = max;
						s = p / 4f;
					}
					else
					{
						s = p / (2f * Mathf.PI) * Mathf.Asin(max / a);
					}

					if (float.IsNaN(s))
					{
						s = 0f;
					}

					if (delta < 1f)
					{
						return -.5f * (a * Mathf.Pow(2f, 10f * (delta -= 1f)) * Mathf.Sin((delta * duration - s) * (2f * Mathf.PI) / p)) + min;
					}
					
					return a * Mathf.Pow(2f, -10f * (delta -= 1f)) *
					       Mathf.Sin((delta * duration - s) * (2f * Mathf.PI) / p) * .5f + max + min;
				}
				
				default:
				{
					return max * delta / duration + min;
				}
			}
		}
	}
}