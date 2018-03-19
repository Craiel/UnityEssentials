namespace Craiel.UnityEssentials.Mathematics.Rnd
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using Contracts;

    public sealed class ConstantDoubleDistribution : DoubleDistribution
    {
        public static readonly ConstantDoubleDistribution Zero = new ConstantDoubleDistribution(0);
        public static readonly ConstantDoubleDistribution NegativeOne = new ConstantDoubleDistribution(-1);
        public static readonly ConstantDoubleDistribution One = new ConstantDoubleDistribution(1);
        public static readonly ConstantDoubleDistribution ZeroPointFive = new ConstantDoubleDistribution(.5f);

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ConstantDoubleDistribution(double value)
        {
            this.Value = value;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public double Value { get; private set; }

        public override double NextDouble()
        {
            return this.Value;
        }

        public override T Clone<T>()
        {
            return (T)(IDistribution)new ConstantDoubleDistribution(this.Value);
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public sealed class ConstantDoubleDistributionAdapter : DistributionAdapters.DoubleAdapter<ConstantDoubleDistribution>
    {
        public ConstantDoubleDistributionAdapter()
            : base("constant")
        {
        }

        protected override ConstantDoubleDistribution ToDistributionTyped(params string[] parameters)
        {
            if (parameters.Length != 1)
            {
                throw new ArgumentException("Expected 1 parameter");
            }

            return new ConstantDoubleDistribution(ParseDouble(parameters[0]));
        }

        protected override string[] ToParametersTyped(ConstantDoubleDistribution distribution)
        {
            return new[] { distribution.Value.ToString(CultureInfo.InvariantCulture) };
        }
    }
}
