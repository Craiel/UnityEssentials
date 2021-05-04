using EssentialMathUtils = Craiel.UnityEssentials.Runtime.Utils.EssentialMathUtils;

namespace Craiel.UnityEssentials.Runtime.Mathematics.Rnd
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using Contracts;

    public class TriangularFloatDistribution : FloatDistribution
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public TriangularFloatDistribution(float high)
            : this(-high, high)
        {
        }

        public TriangularFloatDistribution(float low, float high)
            : this(low, high, (low + high) * .5f)
        {
        }

        public TriangularFloatDistribution(float low, float high, float mode)
        {
            this.Low = low;
            this.High = high;
            this.Mode = mode;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public float Low { get; private set; }

        public float High { get; private set; }

        public float Mode { get; private set; }

        public override float NextFloat()
        {
            if (Math.Abs(-this.Low - this.High) < EssentialMathUtils.DoubleEpsilon && Math.Abs(this.Mode) < EssentialMathUtils.DoubleEpsilon)
            {
                // Faster
                return EssentialMathUtils.RandomTriangular(this.High);
            }

            return EssentialMathUtils.RandomTriangular(this.Low, this.High, this.Mode);
        }

        public override T Clone<T>()
        {
            return (T)(IDistribution)new TriangularFloatDistribution(this.Low, this.High, this.Mode);
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public sealed class TriangularFloatDistributionAdapter : DistributionAdapters.FloatAdapter<TriangularFloatDistribution>
    {
        public TriangularFloatDistributionAdapter()
            : base("triangular")
        {
        }

        protected override TriangularFloatDistribution ToDistributionTyped(params string[] parameters)
        {
            switch (parameters.Length)
            {
                case 1:
                    {
                        return new TriangularFloatDistribution(ParseFloat(parameters[0]));
                    }

                case 2:
                    {
                        return new TriangularFloatDistribution(ParseFloat(parameters[0]), ParseFloat(parameters[1]));
                    }

                case 3:
                    {
                        return new TriangularFloatDistribution(ParseFloat(parameters[0]), ParseFloat(parameters[1]), ParseFloat(parameters[2]));
                    }
            }

            throw new ArgumentException("Expected 1-3 parameters");
        }

        protected override string[] ToParametersTyped(TriangularFloatDistribution distribution)
        {
            return new[] { distribution.Low.ToString(CultureInfo.InvariantCulture), distribution.High.ToString(CultureInfo.InvariantCulture), distribution.Mode.ToString(CultureInfo.InvariantCulture) };
        }
    }
}
