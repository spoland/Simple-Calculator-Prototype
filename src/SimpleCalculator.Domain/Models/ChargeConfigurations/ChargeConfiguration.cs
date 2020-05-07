using SimpleCalculator.Domain.Enums;
using SimpleCalculator.Domain.Options;
using SimpleCalculator.Domain.ValueObjects;
using System;

namespace SimpleCalculator.Domain.Models.ChargeConfigurations
{
    public abstract class ChargeConfiguration
    {
        public ChargeConfiguration(ChargeConfigurationOptions options)
        {
            Name = options.Name;
            CalculationType = options.CalculationType ?? throw new ArgumentException("Base charge type must be specified", nameof(options.CalculationType));
            DeminimisThreshold = options.DeminimisThreshold;

            if (!string.IsNullOrWhiteSpace(options.MinimumPayable))
                MinimumPayable = options.MinimumPayable;

            if (!string.IsNullOrWhiteSpace(options.MinimumCollectible))
                MinimumCollectible = options.MinimumCollectible;
        }

        /// <summary>
        /// The charge name being configured.
        /// </summary>
        public ChargeName Name { get; }

        /// <summary>
        /// The required calculation type - this determines what kind of calculation
        /// this charge will require (weight, value etc.).
        /// </summary>
        public CalculationType CalculationType { get; }

        /// <summary>
        /// Determines at what point this charge should come into affect.
        /// </summary>
        public Price DeminimisThreshold { get; }

        /// <summary>
        /// Determines the minimum amount that must be paid.
        /// </summary>
        public Price? MinimumPayable { get; } = null;

        /// <summary>
        /// Determines the minimum amount that can be collected.
        /// </summary>
        public Price? MinimumCollectible { get; } = null;

        /// <summary>
        /// Determines if the charge value can be easily resolved.
        /// </summary>
        public abstract bool KnownCharge { get; }
    }
}