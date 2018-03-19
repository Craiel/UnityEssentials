using EssentialMathUtils = Craiel.UnityEssentials.Utils.EssentialMathUtils;

namespace Craiel.UnityEssentials.Mathematics.Rnd
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using Contracts;

    public class TriangularLongDistribution : LongDistribution
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public TriangularLongDistribution(long high)
            : this(-high, high)
        {
        }

        public TriangularLongDistribution(long low, long high)
            : this(low, high, (low + high) * .5)
        {
        }

        public TriangularLongDistribution(long low, long high, double mode)
        {
            this.Low = low;
            this.High = high;
            this.Mode = mode;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public long Low { get; private set; }

        public long High { get; private set; }

        public double Mode { get; private set; }

        public override long NextLong()
        {
            if (Math.Abs(-this.Low - this.High) < EssentialMathUtils.DoubleEpsilon && Math.Abs(this.Mode) < EssentialMathUtils.DoubleEpsilon)
            {
                // Faster
                return (long)Math.Round(EssentialMathUtils.RandomTriangular(this.High));
            }

            return (long)Math.Round(EssentialMathUtils.RandomTriangular(this.Low, this.High, this.Mode));
        }

        public override T Clone<T>()
        {
            return (T)(IDistribution)new TriangularLongDistribution(this.Low, this.High, this.Mode);
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public sealed class TriangularLongDistributionAdapter : DistributionAdapters.LongAdapter<TriangularLongDistribution>
    {
        public TriangularLongDistributionAdapter()
            : base("triangular")
        {
        }

        protected override TriangularLongDistribution ToDistributionTyped(params string[] parameters)
        {
            switch (parameters.Length)
            {
                case 1:
                    {
                        return new TriangularLongDistribution(ParseLong(parameters[0]));
                    }

                case 2:
                    {
                        return new TriangularLongDistribution(ParseLong(parameters[0]), ParseLong(parameters[1]));
                    }

                case 3:
                    {
                        return new TriangularLongDistribution(ParseLong(parameters[0]), ParseLong(parameters[1]), ParseDouble(parameters[2]));
                    }
            }

            throw new ArgumentException("Expected 1-3 parameters");
        }

        protected override string[] ToParametersTyped(TriangularLongDistribution distribution)
        {
            return new[] { distribution.Low.ToString(CultureInfo.InvariantCulture), distribution.High.ToString(CultureInfo.InvariantCulture), distribution.Mode.ToString(CultureInfo.InvariantCulture) };
        }
    }
}
