namespace Craiel.UnityEssentials.Mathematics.Rnd
{
    using System;
    using System.Collections.Generic;
    using Contracts;

    public static class DistributionAdapters
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        static DistributionAdapters()
        {
            Adapters.Add(typeof(ConstantDoubleDistribution), new ConstantDoubleDistributionAdapter());
            Adapters.Add(typeof(ConstantFloatDistribution), new ConstantFloatDistributionAdapter());
            Adapters.Add(typeof(ConstantIntegerDistribution), new ConstantIntegerDistributionAdapter());
            Adapters.Add(typeof(ConstantLongDistribution), new ConstantLongDistributionAdapter());

            Adapters.Add(typeof(GaussianDoubleDistribution), new GaussianDoubleDistributionAdapter());
            Adapters.Add(typeof(GaussianFloatDistribution), new GaussianFloatDistributionAdapter());

            Adapters.Add(typeof(TriangularDoubleDistribution), new TriangularDoubleDistributionAdapter());
            Adapters.Add(typeof(TriangularFloatDistribution), new TriangularFloatDistributionAdapter());
            Adapters.Add(typeof(TriangularIntegerDistribution), new TriangularIntegerDistributionAdapter());
            Adapters.Add(typeof(TriangularLongDistribution), new TriangularLongDistributionAdapter());

            Adapters.Add(typeof(UniformDoubleDistribution), new UniformDoubleDistributionAdapter());
            Adapters.Add(typeof(UniformFloatDistribution), new UniformFloatDistributionAdapter());
            Adapters.Add(typeof(UniformIntegerDistribution), new UniformIntegerDistributionAdapter());
            Adapters.Add(typeof(UniformLongDistribution), new UniformLongDistributionAdapter());
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static readonly IDictionary<Type, DistributionAdapter> Adapters = new Dictionary<Type, DistributionAdapter>();

        public abstract class DoubleAdapter<TN> : DistributionAdapter
            where TN : DoubleDistribution
        {
            protected DoubleAdapter(string category)
                : base(category, typeof(DoubleDistribution))
            {
            }

            public override T ToDistribution<T>(params string[] parameters)
            {
                return (T)(IDistribution)this.ToDistributionTyped(parameters);
            }

            public override string[] ToParameters(IDistribution distribution)
            {
                return this.ToParametersTyped((TN)distribution);
            }

            protected abstract TN ToDistributionTyped(params string[] parameters);

            protected abstract string[] ToParametersTyped(TN distribution);
        }

        public abstract class FloatAdapter<TN> : DistributionAdapter
            where TN : FloatDistribution
        {
            protected FloatAdapter(string category)
                : base(category, typeof(FloatDistribution))
            {
            }

            public override T ToDistribution<T>(params string[] parameters)
            {
                return (T)(IDistribution)this.ToDistributionTyped(parameters);
            }

            public override string[] ToParameters(IDistribution distribution)
            {
                return this.ToParametersTyped((TN)distribution);
            }

            protected abstract TN ToDistributionTyped(params string[] parameters);

            protected abstract string[] ToParametersTyped(TN distribution);
        }

        public abstract class IntegerAdapter<TN> : DistributionAdapter
            where TN : IntegerDistribution
        {
            protected IntegerAdapter(string category)
                : base(category, typeof(IntegerDistribution))
            {
            }

            public override T ToDistribution<T>(params string[] parameters)
            {
                return (T)(IDistribution)this.ToDistributionTyped(parameters);
            }

            public override string[] ToParameters(IDistribution distribution)
            {
                return this.ToParametersTyped((TN)distribution);
            }

            protected abstract TN ToDistributionTyped(params string[] parameters);

            protected abstract string[] ToParametersTyped(TN distribution);
        }

        public abstract class LongAdapter<TN> : DistributionAdapter
            where TN : LongDistribution
        {
            protected LongAdapter(string category)
                : base(category, typeof(LongDistribution))
            {
            }

            public override T ToDistribution<T>(params string[] parameters)
            {
                return (T)(IDistribution)this.ToDistributionTyped(parameters);
            }

            public override string[] ToParameters(IDistribution distribution)
            {
                return this.ToParametersTyped((TN)distribution);
            }

            protected abstract TN ToDistributionTyped(params string[] parameters);

            protected abstract string[] ToParametersTyped(TN distribution);
        }
    }
}
