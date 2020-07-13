using SimpleCalculator.Domain.Enums;
using SimpleCalculator.Domain.ValueObjects;

namespace SimpleCalculator.Domain.Models.ChargeConfigurations
{
    public class WeightBasedChargeConfiguration : ChargeConfiguration
    {

        public WeightBasedChargeConfiguration(
            ChargeName chargeName,
            Price deminimisThreshold,
            Rate rate,
            Price? minimumPayable,
            Price? minimumCollectible) : base(chargeName, CalculationType.WeightBased, deminimisThreshold, true, minimumPayable, minimumCollectible)
        {
            Rate = rate;
        }

        /// <summary>
        /// The rate to be applied when calculating the charge.
        /// </summary>
        public Rate Rate { get; }
    }
}
