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

                if (options.FixedChargeAmount == default)
                    throw new InvalidChargeConfigurationException("Fixed charge amount must be provided for fixed charge types.");

                return new FixedRateChargeConfiguration(
                    options.ChargeName,
                    options.DeminimisThreshold,
                    options.FixedChargeAmount);
            }

            if (options.CalculationType == CalculationType.WeightBased)
            {
                if (options.Rate == default)
                    throw new InvalidChargeConfigurationException($"Weight based charges require a {nameof(options.Rate)} value to be set.");

                return new WeightBasedChargeConfiguration(
                    options.ChargeName,
                    options.DeminimisThreshold,
                    options.Rate.GetValueOrDefault(),
                    options.MinimumPayable == null ? default : new Price(options.MinimumPayable),
                    options.MinimumCollectible == null ? default : new Price(options.MinimumCollectible));
            }

            if (options.CalculationType == CalculationType.RateBased)
            {
                if (!options.BaseCharges.Any())
                    throw new InvalidChargeConfigurationException("Rate based charges need to have at least one base charge to be applied on top of.");

                return new RateBasedChargeConfiguration(
                    options.ChargeName,
                    options.DeminimisThreshold,
                    options.BaseCharges.Select(bc => new ChargeName(bc)),
                    options.Rate == null ? default : new Rate(options.Rate.GetValueOrDefault()),
                    options.MinimumPayable == null ? default : new Price(options.MinimumPayable),
                    options.MinimumCollectible == null ? default : new Price(options.MinimumCollectible));
            }

            throw new ArgumentException("Unknown charge type", nameof(options.CalculationType));
        }
    }
}
