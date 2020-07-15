using SimpleCalculator.Domain.Abstractions;
using SimpleCalculator.Domain.Constants;
using SimpleCalculator.Domain.Entities;
using SimpleCalculator.Domain.ValueObjects;

namespace SimpleCalculator.Domain.Calculators
{
    public class FixedChargeCalculator : IChargeCalculator
    {
        private readonly ChargeName _chargeName;
        private readonly Price _amount;

        public FixedChargeCalculator(ChargeName chargeName, Price amount)
        {
            _chargeName = chargeName;
            _amount = amount;
        }

        public void Calculate(Order order)
        {
            foreach (var item in order.OrderItems)
            {
                var chargeAmount = _amount * order.RelativeOrderItemValue(item);

                var chargeName = ChargeName.FromBaseChargeName(_chargeName, ChargeNames.Item);

                item.AddCharge(new OrderCharge(chargeName, chargeAmount, _chargeName));
            }
        }
    }
}
