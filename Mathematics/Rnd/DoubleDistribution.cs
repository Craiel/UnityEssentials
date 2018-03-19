namespace Craiel.UnityEssentials.Mathematics.Rnd
{
    using Contracts;

    public abstract class DoubleDistribution : IDistribution
    {
        public int NextInt()
        {
            return (int)this.NextDouble();
        }

        public long NextLong()
        {
            return (long)this.NextDouble();
        }

        public float NextFloat()
        {
            return (float)this.NextDouble();
        }

        public abstract double NextDouble();

        public abstract T Clone<T>() where T : IDistribution;
    }
}
