namespace Craiel.UnityEssentials.Runtime.Mathematics.Rnd
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using Contracts;

    public sealed class ConstantFloatDistribution : FloatDistribution
    {
        public static readonly ConstantFloatDistribution Zero = new ConstantFloatDistribution(0);
        public static readonly ConstantFloatDistribution NegativeOne = new ConstantFloatDistribution(-1);
        public static readonly ConstantFloatDistribution One = new ConstantFloatDistribution(1);
        public static readonly ConstantFloatDistribution ZeroPointFive = new ConstantFloatDistribution(.5f);

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ConstantFloatDistribution(float value)
        {
            this.Value = value;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public float Value { get; private set; }

        public override float NextFloat()
        {
            return this.Value;
        }

        public override T Clone<T>()
        {
            return (T)(IDistribution)new ConstantFloatDistribution(this.Value);
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public sealed class ConstantFloatDistributionAdapter : DistributionAdapters.FloatAdapter<ConstantFloatDistribution>
    {
        public ConstantFloatDistributionAdapter()
            : base("constant")
        {
        }

        protected override ConstantFloatDistribution ToDistributionTyped(params string[] parameters)
        {
            if (parameters.Length != 1)
            {
                throw new ArgumentException("Expected 1 parameter");
            }

            return new ConstantFloatDistribution(ParseFloat(parameters[0]));
        }

        protected override string[] ToParametersTyped(ConstantFloatDistribution distribution)
        {
            return new[] { distribution.Value.ToString(CultureInfo.InvariantCulture) };
        }
    }
}
