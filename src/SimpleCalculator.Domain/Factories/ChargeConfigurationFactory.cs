using SimpleCalculator.Domain.Enums;
using SimpleCalculator.Domain.Models.ChargeConfigurations;
using SimpleCalculator.Domain.Options;
using System;

namespace SimpleCalculator.Domain.Factories
{
    public static class ChargeConfigurationFactory
    {
        public static ChargeConfiguration CreateFromOptions(ChargeConfigurationOptions options)
        {
            if (options.CalculationType == CalculationType.Fixed)
                return new FixedRateChargeConfiguration(options);

            if (options.CalculationType == CalculationType.RateBased)
                return new RateBasedChargeConfiguration(options);

            if (options.CalculationType == CalculationType.WeightBased)
                return new WeightBasedChargeConfiguration(options);

            throw new ArgumentException("Unknown charge type", nameof(options.CalculationType));
        }
    }
}
