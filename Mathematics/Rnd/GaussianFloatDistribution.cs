using EssentialMathUtils = Craiel.UnityEssentials.Utils.EssentialMathUtils;

namespace Craiel.UnityEssentials.Mathematics.Rnd
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using Contracts;

    public class GaussianFloatDistribution : FloatDistribution
    {
        public static readonly GaussianFloatDistribution StandardNormal = new GaussianFloatDistribution(0, 1);

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public GaussianFloatDistribution(float mean, float standardDeviation)
        {
            this.Mean = mean;
            this.StandardDeviation = standardDeviation;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public float Mean { get; private set; }

        public float StandardDeviation { get; private set; }

        public override float NextFloat()
        {
            return this.Mean + (float)EssentialMathUtils.NextGaussian() * this.StandardDeviation;
        }

        public override T Clone<T>()
        {
            return (T)(IDistribution)new GaussianFloatDistribution(this.Mean, this.StandardDeviation);
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public sealed class GaussianFloatDistributionAdapter : DistributionAdapters.FloatAdapter<GaussianFloatDistribution>
    {
        public GaussianFloatDistributionAdapter()
            : base("gaussian")
        {
        }

        protected override GaussianFloatDistribution ToDistributionTyped(params string[] parameters)
        {
            if (parameters.Length != 2)
            {
                throw new ArgumentException("Expected 2 parameters");
            }

            return new GaussianFloatDistribution(ParseFloat(parameters[0]), ParseFloat(parameters[1]));
        }

        protected override string[] ToParametersTyped(GaussianFloatDistribution distribution)
        {
            return new[] { distribution.Mean.ToString(CultureInfo.InvariantCulture), distribution.StandardDeviation.ToString(CultureInfo.InvariantCulture) };
        }
    }
}
