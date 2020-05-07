using SimpleCalculator.Domain.Exceptions;
using SimpleCalculator.Domain.Options;
using SimpleCalculator.Domain.ValueObjects;
using System.Linq;

namespace SimpleCalculator.Domain.Models.ChargeConfigurations
{
    public class FixedRateChargeConfiguration : ChargeConfiguration
    {
        public FixedRateChargeConfiguration(ChargeConfigurationOptions options) : base(options)
        {
            if (options.BaseCharges.Any())
                throw new InvalidChargeConfigurationException("Fixed charges should not have any dependencies.");

            FixedChargeAmount = options.FixedChargeAmount ?? throw new InvalidChargeConfigurationException("Fixed charges require a fixed charge amount to be specified.");
        }

        /// <summary>
        /// The fixed charge amount to be paid.
        /// </summary>
        public Price FixedChargeAmount { get; }

        public override bool KnownCharge => true;
    }
}
