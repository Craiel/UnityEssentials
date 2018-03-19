namespace Craiel.UnityEssentials.Mathematics.Rnd
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using Contracts;

    public class UniformLongDistribution : LongDistribution
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public UniformLongDistribution(long high)
            : this(0, high)
        {
        }

        public UniformLongDistribution(long low, long high)
        {
            this.Low = low;
            this.High = high;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public long Low { get; private set; }

        public long High { get; private set; }

        public override long NextLong()
        {
            return this.Low + (long)(UnityEngine.Random.value * (this.High - this.Low));
        }

        public override T Clone<T>()
        {
            return (T)(IDistribution)new UniformLongDistribution(this.Low, this.High);
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public sealed class UniformLongDistributionAdapter : DistributionAdapters.LongAdapter<UniformLongDistribution>
    {
        public UniformLongDistributionAdapter()
            : base("uniform")
        {
        }

        protected override UniformLongDistribution ToDistributionTyped(params string[] parameters)
        {
            switch (parameters.Length)
            {
                case 1:
                    {
                        return new UniformLongDistribution(ParseLong(parameters[0]));
                    }

                case 2:
                    {
                        return new UniformLongDistribution(ParseLong(parameters[0]), ParseLong(parameters[1]));
                    }
            }

            throw new ArgumentException("Expected 1-2 parameters");
        }

        protected override string[] ToParametersTyped(UniformLongDistribution distribution)
        {
            return new[] { distribution.Low.ToString(CultureInfo.InvariantCulture), distribution.High.ToString(CultureInfo.InvariantCulture) };
        }
    }
}
