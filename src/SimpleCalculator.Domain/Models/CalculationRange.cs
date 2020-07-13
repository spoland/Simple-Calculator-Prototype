using SimpleCalculator.Domain.Exceptions;
using SimpleCalculator.Domain.Models.ChargeConfigurations;
using SimpleCalculator.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleCalculator.Domain.Models
{
    public class CalculationRange
    {
        public CalculationRange(Price deminimisThreshold, IEnumerable<ChargeConfiguration> configurations)
        {
            DeminimisThreshold = deminimisThreshold;
            ChargeConfigurations = configurations;
        }

        /// <summary>
        /// The deminimis threshold for this range
        /// </summary>
        public Price DeminimisThreshold { get; }

        /// <summary>
        /// The list of charge configurations that exist in this calculation range.
        /// </summary>
        public IEnumerable<ChargeConfiguration> ChargeConfigurations { get; private set; }

        /// <summary>
        /// Topologically sort the charge configuration graph.
        /// </summary>
        public void Sort()
        {
            ChargeConfigurations = TopologicalSort(ChargeConfigurations.OrderByDescending(x => x.KnownCharge).ThenBy(x => x.ChargeName.Value), GetDependencies);
        }

        /// <summary>
        /// Returns a collection of dependencies for a charge configuration instance.
        /// Returns an empty collection unless the configuration is a <see cref="RateBasedChargeConfiguration"/>.
        /// This is because other configuration types (weight, fixed) do not depend on any other configuration
        /// as they know how to calculate charges.
        /// </summary>
        private IEnumerable<ChargeConfiguration> GetDependencies(ChargeConfiguration config) =>
            config is RateBasedChargeConfiguration ? ((RateBasedChargeConfiguration)config).BaseCharges : Enumerable.Empty<ChargeConfiguration>();

        /// <summary>
        /// A topological sorting algorithm used to determine the order in which calculations must be executed.
        /// </summary>
        private IList<T> TopologicalSort<T>(IEnumerable<T> source, Func<T, IEnumerable<T>> getDependencies)
        {
            var sorted = new List<T>();
            var visited = new Dictionary<T, bool>();

            foreach (var item in source)
            {
                Visit(item, getDependencies, sorted, visited);
            }

            return sorted;
        }

        private void Visit<T>(T item, Func<T, IEnumerable<T>> getDependencies, List<T> sorted, Dictionary<T, bool> visited)
        {
            bool inProcess;
            var alreadyVisited = visited.TryGetValue(item, out inProcess);

            if (alreadyVisited)
            {
                if (inProcess)
                {
                    throw new InvalidChargeConfigurationException("Cyclic dependencies exist between charge configurations.");
                }
            }
            else
            {
                visited[item] = true;

                var dependencies = getDependencies(item);
                if (dependencies != null)
                {
                    foreach (var dependency in dependencies)
                    {
                        Visit(dependency, getDependencies, sorted, visited);
                    }
                }

                visited[item] = false;
                sorted.Add(item);
            }
        }
    }
}
