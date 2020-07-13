using SimpleCalculator.Domain.ValueObjects;
using System.Collections.Generic;
using System.Linq;
using SimpleCalculator.Domain.Enums;

namespace SimpleCalculator.Domain.Models.ChargeConfigurations
{
    public class RateBasedChargeConfiguration : ChargeConfiguration
    {
        private readonly List<ChargeName> _baseChargeNames;

        public RateBasedChargeConfiguration(
            ChargeName chargeName,
            Price deminimisThreshold,            
            IEnumerable<ChargeName> baseChargeNames,
            Rate? rate = default,
            Price? minimumPayable = default,
            Price? minimumCollectible = default) : base(chargeName, CalculationType.RateBased, deminimisThreshold, false, minimumPayable, minimumCollectible)
        {
            Rate = rate;
            _baseChargeNames = baseChargeNames.ToList();
        }

        /// <summary>
        /// Determines what charge types this charge should be applied
        /// on top of. These charges must be calculated first.
        /// </summary>
        public IEnumerable<ChargeConfiguration> BaseCharges { get; private set; } = Enumerable.Empty<ChargeConfiguration>();

        /// <summary>
        /// Set the charge configurations that this charge depends on
        /// </summary>
        /// <param name="configurations">A list of all configurations that are found in the same deminimis region.</param>
        public void SetDependencies(IEnumerable<ChargeConfiguration> configurations)
        {
            BaseCharges = configurations.Where(c => _baseChargeNames.Contains(c.ChargeName.Value));
        }

        /// <summary>
        /// The rate that should be applied - can be null as the rate may be provided on the item.
        /// </summary>
        public Rate? Rate { get; }
    }
}
