using SimpleCalculator.Api.Options;
using SimpleCalculator.Domain.Enums;
using SimpleCalculator.Domain.Exceptions;
using SimpleCalculator.Domain.Models.ChargeConfigurations;
using SimpleCalculator.Domain.ValueObjects;
using System;
using System.Linq;

namespace SimpleCalculator.Api.Factories
{
    public static class ChargeConfigurationFactory
    {
        public static ChargeConfiguration CreateFromOptions(ChargeConfigurationOptions options)
        {
            if (options.CalculationType == CalculationType.Fixed)
            {
                if (options.BaseCharges.Any())
                    throw new InvalidChargeConfigurationException("Fixed charges should not have any dependencies.");

                if (options.FixedChargeAmount == null)
                    throw new InvalidChargeConfigurationException("Fixed charge amount must be provided for fixed charge types.");

                return new FixedRateChargeConfiguration(
                    options.ChargeName,
                    options.DeminimisThreshold,
                    options.FixedChargeAmount);
            }

            if (options.CalculationType == CalculationType.WeightBased)
            {
                return new WeightBasedChargeConfiguration(
                    options.ChargeName,
                    options.DeminimisThreshold,
                    options.Rate == null ? null as Rate : new Rate(options.Rate),
                    options.MinimumPayable ?? null,
                    options.MinimumCollectible ?? null);
            }
                


            if (options.CalculationType == CalculationType.RateBased)
                return new RateBasedChargeConfiguration(options);



            throw new ArgumentException("Unknown charge type", nameof(options.CalculationType));
        }
    }
}
