namespace Craiel.UnityEssentials.Runtime.Mathematics.Rnd
{
    using Contracts;

    public abstract class LongDistribution : IDistribution
    {
        public int NextInt()
        {
            return (int)this.NextLong();
        }

        public abstract long NextLong();

        public float NextFloat()
        {
            return this.NextLong();
        }

        public double NextDouble()
        {
            return this.NextLong();
        }

        public abstract T Clone<T>() where T : IDistribution;
    }
}
