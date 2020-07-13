using SimpleCalculator.Domain.Enums;
using SimpleCalculator.Domain.ValueObjects;

namespace SimpleCalculator.Domain.Models.ChargeConfigurations
{
    public class FixedRateChargeConfiguration : ChargeConfiguration
    {
        public FixedRateChargeConfiguration(
            ChargeName chargeName,
            Price deminimisThreshold,
            Price fixedChargeAmount) : base(chargeName, CalculationType.Fixed, deminimisThreshold, true, null, null)
        {
            FixedChargeAmount = fixedChargeAmount;
        }

        /// <summary>
        /// The fixed charge amount to be paid.
        /// </summary>
        public Price FixedChargeAmount { get; }
    }
}
