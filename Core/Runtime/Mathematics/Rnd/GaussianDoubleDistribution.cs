using EssentialMathUtils = Craiel.UnityEssentials.Runtime.Utils.EssentialMathUtils;

namespace Craiel.UnityEssentials.Runtime.Mathematics.Rnd
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using Contracts;

    public class GaussianDoubleDistribution : DoubleDistribution
    {
        public static readonly GaussianDoubleDistribution StandardNormal = new GaussianDoubleDistribution(0, 1);

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public GaussianDoubleDistribution(double mean, double standardDeviation)
        {
            this.Mean = mean;
            this.StandardDeviation = standardDeviation;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public double Mean { get; private set; }

        public double StandardDeviation { get; private set; }

        public override double NextDouble()
        {
            return this.Mean + EssentialMathUtils.NextGaussian() * this.StandardDeviation;
        }

        public override T Clone<T>()
        {
            return (T)(IDistribution)new GaussianDoubleDistribution(this.Mean, this.StandardDeviation);
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public sealed class GaussianDoubleDistributionAdapter : DistributionAdapters.DoubleAdapter<GaussianDoubleDistribution>
    {
        public GaussianDoubleDistributionAdapter()
            : base("gaussian")
        {
        }

        protected override GaussianDoubleDistribution ToDistributionTyped(params string[] parameters)
        {
            if (parameters.Length != 2)
            {
                throw new ArgumentException("Expected 2 parameters");
            }

            return new GaussianDoubleDistribution(ParseDouble(parameters[0]), ParseDouble(parameters[1]));
        }

        protected override string[] ToParametersTyped(GaussianDoubleDistribution distribution)
        {
            return new[] { distribution.Mean.ToString(CultureInfo.InvariantCulture), distribution.StandardDeviation.ToString(CultureInfo.InvariantCulture) };
        }
    }
}
