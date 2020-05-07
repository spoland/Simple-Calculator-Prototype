using SimpleCalculator.Domain.Models;
using SimpleCalculator.Domain.ValueObjects;

namespace SimpleCalculator.Domain.Calculators.Constraints
{
    public class MinimumPayableConstraint : IChargeCalculator
    {
        private readonly ChargeName _chargeName;
        private readonly Price _minimumPayable;
        private readonly IChargeCalculator _chargeCalculator;
        
        public MinimumPayableConstraint(
            IChargeCalculator chargeCalculator,
            ChargeName chargeName,
            Price minimumPayable)
        {
            _chargeCalculator = chargeCalculator;
            _chargeName = chargeName;
            _minimumPayable = minimumPayable;
        }

        public void Calculate(Order order)
        {
            _chargeCalculator.Calculate(order);

            var appliedCharge = order.GetTotalCharge(_chargeName);

            if (appliedCharge.Charge < _minimumPayable)
            {
                foreach(var item in order.OrderItems)
                {
                    item.RemoveCharge(_chargeName);

                    var minimumItemCharge = _minimumPayable * order.RelativeItemValue(item).AsDecimal;

                    item.AddCharge(new OrderCharge(_chargeName, minimumItemCharge, _chargeName));
                }
            }
        }
    }
}