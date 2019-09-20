namespace Craiel.UnityEssentials.Runtime.Mathematics.Rnd
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
            Adapters.Add(TypeCache<ConstantDoubleDistribution>.Value, new ConstantDoubleDistributionAdapter());
            Adapters.Add(TypeCache<ConstantFloatDistribution>.Value, new ConstantFloatDistributionAdapter());
            Adapters.Add(TypeCache<ConstantIntegerDistribution>.Value, new ConstantIntegerDistributionAdapter());
            Adapters.Add(TypeCache<ConstantLongDistribution>.Value, new ConstantLongDistributionAdapter());

            Adapters.Add(TypeCache<GaussianDoubleDistribution>.Value, new GaussianDoubleDistributionAdapter());
            Adapters.Add(TypeCache<GaussianFloatDistribution>.Value, new GaussianFloatDistributionAdapter());

            Adapters.Add(TypeCache<TriangularDoubleDistribution>.Value, new TriangularDoubleDistributionAdapter());
            Adapters.Add(TypeCache<TriangularFloatDistribution>.Value, new TriangularFloatDistributionAdapter());
            Adapters.Add(TypeCache<TriangularIntegerDistribution>.Value, new TriangularIntegerDistributionAdapter());
            Adapters.Add(TypeCache<TriangularLongDistribution>.Value, new TriangularLongDistributionAdapter());

            Adapters.Add(TypeCache<UniformDoubleDistribution>.Value, new UniformDoubleDistributionAdapter());
            Adapters.Add(TypeCache<UniformFloatDistribution>.Value, new UniformFloatDistributionAdapter());
            Adapters.Add(TypeCache<UniformIntegerDistribution>.Value, new UniformIntegerDistributionAdapter());
            Adapters.Add(TypeCache<UniformLongDistribution>.Value, new UniformLongDistributionAdapter());
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static readonly IDictionary<Type, DistributionAdapter> Adapters = new Dictionary<Type, DistributionAdapter>();

        public abstract class DoubleAdapter<TN> : DistributionAdapter
            where TN : DoubleDistribution
        {
            protected DoubleAdapter(string category)
                : base(category, TypeCache<DoubleDistribution>.Value)
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
                : base(category, TypeCache<FloatDistribution>.Value)
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
                : base(category, TypeCache<IntegerDistribution>.Value)
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
                : base(category, TypeCache<LongDistribution>.Value)
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
