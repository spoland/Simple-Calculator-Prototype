using SimpleCalculator.Domain.Exceptions;
using SimpleCalculator.Domain.Options;
using SimpleCalculator.Domain.ValueObjects;
using System;
using System.Linq;

namespace SimpleCalculator.Domain.Models.ChargeConfigurations
{
    public class WeightBasedChargeConfiguration : ChargeConfiguration
    {
        public WeightBasedChargeConfiguration(ChargeConfigurationOptions options) : base(options)
        {
            if (options.BaseCharges.Any())
                throw new InvalidChargeConfigurationException("Weight based charges should not have any dependencies.");

            Rate = options.Rate ?? throw new ArgumentException("A weight based calculator requires a rate to be set.");
        }

        /// <summary>
        /// The rate to be applied when calculating the charge.
        /// </summary>
        public Rate Rate { get; }

        public override bool KnownCharge => true;
    }
}
