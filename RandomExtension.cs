namespace Assets.Scripts.Craiel.Essentials
{
    using System;
    using System.Collections.Generic;

    public static class RandomExtension
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static long NextLong(this Random rand)
        {
            return rand.NextLong(long.MinValue, long.MaxValue);
        }

        public static long NextLong(this Random rand, long min)
        {
            return rand.NextLong(min, long.MaxValue);
        }

        public static long NextLong(this Random rand, long min, long max)
        {
            long result = rand.Next((int)min >> 32, (int)max >> 32);
            result = result << 32;
            result = result | rand.Next((int)min, (int)max);
            return result;
        }

        public static float Range(this Random rand, float min, float max)
        {
            return (float)(min + (rand.NextDouble() * (max - min)));
        }

        public static int WeightedRandom(IList<float> weights)
        {
            // sum the weights
            float total = 0;
            foreach (float weight in weights)
            {
                total += weight;
            }

            // select a random value between 0 and our total
            float random = UnityEngine.Random.Range(0, total);

            // loop thru our weights until we arrive at the correct one
            float current = 0;
            for (int i = 0; i < weights.Count; ++i)
            {
                current += weights[i];
                if (random <= current)
                {
                    return i;
                }
            }

            // shouldn't happen.
            throw new InvalidOperationException("WeightedRandom has reached an unkwnon outcome");
        }

        public static T WeightedRandom<T>(IList<T> values, IList<float> weights)
        {
            if (values.Count != weights.Count)
            {
                return default(T);
            }

            int rollIndex = WeightedRandom(weights);
            return values[rollIndex];
        }

        public static int WeightedRandom(this Random rand, IList<float> weights)
        {
            // sum the weights
            float total = 0;
            foreach (float weight in weights)
            {
                total += weight;
            }

            // select a random value between 0 and our total
            float random = rand.Range(0, total);

            // loop thru our weights until we arrive at the correct one
            float current = 0;
            for (int i = 0; i < weights.Count; ++i)
            {
                current += weights[i];
                if (random <= current)
                {
                    return i;
                }
            }

            // shouldn't happen.
            throw new InvalidOperationException("WeightedRandom has reached an unkwnon outcome");
        }

        public static T WeightedRandom<T>(this Random rand, IList<T> values, IList<float> weights)
        {
            if (values.Count != weights.Count)
            {
                return default(T);
            }

            int rollIndex = rand.WeightedRandom(weights);
            return values[rollIndex];
        }
    }
}
