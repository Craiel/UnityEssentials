namespace Craiel.UnityEssentials.Mathematics.Rnd
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Contracts;

    public class DistributionAdapterHandler
    {
        private readonly IDictionary<Type, DistributionAdapter> map;
        private readonly IDictionary<Type, IDictionary<string, DistributionAdapter>> typeMap;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public DistributionAdapterHandler()
        {
            this.map = new Dictionary<Type, DistributionAdapter>();
            this.typeMap = new Dictionary<Type, IDictionary<string, DistributionAdapter>>();

            foreach (KeyValuePair<Type, DistributionAdapter> pair in DistributionAdapters.Adapters)
            {
                this.Add(pair.Key, pair.Value);
            }
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Add(Type type, DistributionAdapter adapter)
        {
            this.map.Add(type, adapter);

            IDictionary<string, DistributionAdapter> typeList;
            if (!this.typeMap.TryGetValue(type, out typeList))
            {
                typeList = new Dictionary<string, DistributionAdapter>();
                this.typeMap.Add(type, typeList);
            }

            typeList.Add(adapter.Category, adapter);
        }

        public T ToDistribution<T>(string value, Type type)
            where T : IDistribution
        {
            string[] parameters = value.Split('\t', '\f');
            if (parameters.Length < 1)
            {
                throw new DistributionFormatException("Missing distribution type");
            }

            IDictionary<string, DistributionAdapter> typeList;
            if (!this.typeMap.TryGetValue(type, out typeList))
            {
                throw new DistributionFormatException(string.Format("No adapter set for type {0}", type));
            }

            DistributionAdapter adapter;
            if (!typeList.TryGetValue(parameters[0], out adapter))
            {
                throw new DistributionFormatException(string.Format("No adapter set for type {0} and category {1}",
                    type, parameters[0]));
            }

            return adapter.ToDistribution<T>(parameters.Skip(1).ToArray());
        }

        public string ToString(IDistribution distribution)
        {
            DistributionAdapter adapter;
            if (!this.map.TryGetValue(distribution.GetType(), out adapter))
            {
                throw new DistributionFormatException(
                    string.Format("No adapter set for type {0}", distribution.GetType()));
            }

            string[] parameters = adapter.ToParameters(distribution);
            
            return string.Concat(adapter.Category, ',', string.Join(",", parameters));
        }
    }
}
