using SimpleCalculator.Domain.Exceptions;
using SimpleCalculator.Domain.Models.ChargeConfigurations;
using SimpleCalculator.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleCalculator.Domain.Models
{
    /// <summary>
    /// Describes all possible combinations of charges that could be required by a calculation.
    /// </summary>
    public class CalculatorConfiguration
    {
        /// <summary>
        /// Creates a new <see cref="CalculatorConfiguration"/> based on a <see cref="CalculatorConfigurationOptions"/> object.
        /// </summary>
        /// <param name="calculatorConfigurationOptions">The calculator configuration options.</param>
        public CalculatorConfiguration(
            IEnumerable<ChargeConfiguration> chargeConfigurations,
            IEnumerable<ChargeName> deminimisBaseCharges)
        {
            if (!chargeConfigurations.Any())
                throw new ArgumentException("At least one configuration must exist in order to perform a calculation.");

            // Group configurations by deminimis threshold
            var configurationGroups = chargeConfigurations
                .GroupBy(x => x.DeminimisThreshold)
                .OrderBy(x => x.Key)
                .ToList();

            // Check for duplicate charge configurations
            configurationGroups.ForEach(group =>
            {
                var groupList = group.Select(x => x.ChargeName).ToList();
                if (groupList.Count != groupList.Distinct().Count())
                    throw new InvalidChargeConfigurationException("Duplicate charge configurations have been specified in the same range.");
            });

            CalculationRanges = CreateRanges(configurationGroups);
            DeminimisBaseCharges = deminimisBaseCharges.Select(chargeName => new ChargeName(chargeName));
        }

        /// <summary>
        /// A collection of <see cref="CalculationRange"/> objects.
        /// </summary>
        public IEnumerable<CalculationRange> CalculationRanges { get; }

        /// <summary>
        /// A collection of the names of the charges which make up the base price for calculations.
        /// </summary>
        public IEnumerable<ChargeName> DeminimisBaseCharges { get; }

        /// <summary>
        /// Get the correct <see cref="CalculationRange"/> for a base price.
        /// </summary>
        /// <param name="basePrice"></param>
        /// <returns></returns>
        public CalculationRange GetRangeForBasePrice(Price basePrice) =>
            CalculationRanges.Reverse().First(x => basePrice >= x.DeminimisThreshold);

        private List<CalculationRange> CreateRanges(List<IGrouping<Price, ChargeConfiguration>> configurationGroups)
        {
            // Create range list and add the first range
            List<CalculationRange> deminimisRanges = new List<CalculationRange>
            {
                new CalculationRange(configurationGroups[0].Key, configurationGroups[0].ToList())
            };

            // Loop through remaining groups starting at position 1
            for (int i = 1; i < configurationGroups.Count; i++)
            {
                var threshold = configurationGroups[i].Key;

                // Start with charge configurations found in last range
                var configs = deminimisRanges[i - 1].ChargeConfigurations.ToList();
                var newConfigs = configurationGroups[i].ToList();

                // Replace charges from last range that exist in this range
                configs.RemoveAll(config => newConfigs.Select(c => c.ChargeName).Contains(config.ChargeName));
                configs.AddRange(newConfigs);

                deminimisRanges.Add(new CalculationRange(threshold, configs));
            }

            // Set dependencies and sort
            foreach (var range in deminimisRanges)
            {
                foreach (var config in range.ChargeConfigurations.OfType<RateBasedChargeConfiguration>())
                {
                    config.SetDependencies(range.ChargeConfigurations);
                }

                range.Sort(); // can only be done after dependencies have been set
            }

            return deminimisRanges;
        }
    }
}