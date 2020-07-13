using SimpleCalculator.Domain.Abstractions;
using SimpleCalculator.Domain.Entities;
using SimpleCalculator.Domain.ValueObjects;

namespace SimpleCalculator.Domain.Calculators.Constraints
{
    public class MinimumCollectibleConstraint : IChargeCalculator
    {
        private readonly IChargeCalculator _chargeCalculator;
        private readonly ChargeName _chargeName;
        private readonly Price _minimumCollectible;

        public MinimumCollectibleConstraint(
            IChargeCalculator chargeCalculator,
            ChargeName chargeName,
            Price minimumCollectible)
        {
            _chargeCalculator = chargeCalculator;
            _chargeName = chargeName;
            _minimumCollectible = minimumCollectible;
        }

        public void Calculate(Order order)
        {
            _chargeCalculator.Calculate(order);

            var appliedCharge = order.GetCharge(_chargeName, order.Currency);

            if (appliedCharge.ChargeAmount < _minimumCollectible)
                order.RemoveCharge(_chargeName);
        }
    }
}