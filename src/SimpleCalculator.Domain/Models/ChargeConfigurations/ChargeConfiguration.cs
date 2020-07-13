using SimpleCalculator.Domain.Enums;
using SimpleCalculator.Domain.ValueObjects;

namespace SimpleCalculator.Domain.Models.ChargeConfigurations
{
    public abstract class ChargeConfiguration
    {
        protected ChargeConfiguration(
            ChargeName chargeName,
            CalculationType calculationType,
            Price deminimisThreshold,
            bool knownCharge,
            Price? minimumPayable,
            Price? minimumCollectible)
        {
            ChargeName = chargeName;
            CalculationType = calculationType;
            DeminimisThreshold = deminimisThreshold;
            MinimumPayable = minimumPayable;
            MinimumCollectible = minimumCollectible;
            KnownCharge = knownCharge;
        }

        /// <summary>
        /// The charge name being configured.
        /// </summary>
        public ChargeName ChargeName { get; }

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
        public bool KnownCharge { get; }
    }
}