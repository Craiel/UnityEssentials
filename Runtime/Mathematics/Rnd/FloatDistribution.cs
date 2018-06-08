namespace Craiel.UnityEssentials.Runtime.Mathematics.Rnd
{
    using Contracts;

    public abstract class FloatDistribution : IDistribution
    {
        public int NextInt()
        {
            return (int)this.NextFloat();
        }

        public long NextLong()
        {
            return (long)this.NextFloat();
        }

        public abstract float NextFloat();

        public double NextDouble()
        {
            return this.NextFloat();
        }

        public abstract T Clone<T>() where T : IDistribution;
    }
}
