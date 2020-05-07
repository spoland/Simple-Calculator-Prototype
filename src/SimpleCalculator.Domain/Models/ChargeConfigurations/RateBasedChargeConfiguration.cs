using SimpleCalculator.Domain.Exceptions;
using SimpleCalculator.Domain.Options;
using SimpleCalculator.Domain.ValueObjects;
using System.Collections.Generic;
using System.Linq;

namespace SimpleCalculator.Domain.Models.ChargeConfigurations
{
    public class RateBasedChargeConfiguration : ChargeConfiguration
    {
        private readonly List<ChargeName> _baseChargeNames;

        public RateBasedChargeConfiguration(ChargeConfigurationOptions options) : base(options)
        {
            if (!options.BaseCharges.Any())
                throw new InvalidChargeConfigurationException("Rate based charges require at least one dependency to be specified.");

            _baseChargeNames = options.BaseCharges.Select(bc => new ChargeName(bc)).ToList();
            if (options.Rate.HasValue) Rate = new Rate(options.Rate.GetValueOrDefault());
        }

        /// <summary>
        /// Determines what charge types this charge should be applied
        /// on top of. These charges must be calculated first.
        /// </summary>
        public IEnumerable<ChargeConfiguration> BaseCharges { get; private set; } = Enumerable.Empty<ChargeConfiguration>();

        /// <summary>
        /// Exposes a list of charge names.
        /// </summary>
        public IEnumerable<ChargeName> BaseChargeNames => _baseChargeNames;

        /// <summary>
        /// Set the charge configurations that this charge depends on
        /// </summary>
        /// <param name="configurations">A list of all configurations that are found in the same deminimis region.</param>
        public void SetDependencies(IEnumerable<ChargeConfiguration> configurations)
        {
            BaseCharges = configurations.Where(c => _baseChargeNames.Contains(c.Name.Value));
        }

        /// <summary>
        /// The rate that should be applied - can be null as the rate may be provided on the item.
        /// </summary>
        public Rate? Rate { get; }

        public override bool KnownCharge => false;
    }
}
