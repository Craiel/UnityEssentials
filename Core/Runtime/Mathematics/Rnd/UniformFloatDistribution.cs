namespace Craiel.UnityEssentials.Runtime.Mathematics.Rnd
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using Contracts;

    public class UniformFloatDistribution : FloatDistribution
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public UniformFloatDistribution(float high)
            : this(0, high)
        {
        }

        public UniformFloatDistribution(float low, float high)
        {
            this.Low = low;
            this.High = high;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public float Low { get; private set; }

        public float High { get; private set; }

        public override float NextFloat()
        {
            return this.Low + (float)(UnityEngine.Random.value * (this.High - this.Low));
        }

        public override T Clone<T>()
        {
            return (T)(IDistribution)new UniformFloatDistribution(this.Low, this.High);
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public sealed class UniformFloatDistributionAdapter : DistributionAdapters.FloatAdapter<UniformFloatDistribution>
    {
        public UniformFloatDistributionAdapter()
            : base("uniform")
        {
        }

        protected override UniformFloatDistribution ToDistributionTyped(params string[] parameters)
        {
            switch (parameters.Length)
            {
                case 1:
                    {
                        return new UniformFloatDistribution(ParseFloat(parameters[0]));
                    }

                case 2:
                    {
                        return new UniformFloatDistribution(ParseFloat(parameters[0]), ParseFloat(parameters[1]));
                    }
            }

            throw new ArgumentException("Expected 1-2 parameters");
        }

        protected override string[] ToParametersTyped(UniformFloatDistribution distribution)
        {
            return new[] { distribution.Low.ToString(CultureInfo.InvariantCulture), distribution.High.ToString(CultureInfo.InvariantCulture) };
        }
    }
}
