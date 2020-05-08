using SimpleCalculator.Domain.Constants;
using SimpleCalculator.Domain.Models;
using SimpleCalculator.Domain.ValueObjects;

namespace SimpleCalculator.Domain.Calculators.Charge
{

    // TODO: Split into two different classes. One for calculating and splitting the excess,
    // one for adding it back to the item (or whatever other) price.

    public class ExcessCalculator : IChargeCalculator
    {
        private readonly Price _excessAmount;
        private bool _excessApplied;

        public ExcessCalculator(Price excessAmount)
        {
            _excessAmount = excessAmount;
        }

        public void Calculate(Order order)
        {
            ChargeName itemChargeName = ChargeNames.Item;
            ChargeName excessChargeName = ChargeNames.Excess;
            ChargeName itemExcessChargeName = ChargeName.FromBaseChargeName(excessChargeName, itemChargeName);

            if (!_excessApplied)
            {
                var totalItemCharge = order.GetCharge(itemChargeName);

                if (totalItemCharge.Charge > _excessAmount)
                {
                    foreach(var item in order.OrderItems)
                    {
                        var itemExcessAmount = _excessAmount * order.RelativeItemValue(item).AsDecimal;
                        var itemExcessCharge = new OrderCharge(itemExcessChargeName, itemExcessAmount, new ChargeName("excess"));

                        var itemCharge = item.GetCharge(itemChargeName);
                        itemCharge.Charge -= itemExcessCharge.Charge;

                        item.AddCharge(itemExcessCharge);
                    }
                }

                _excessApplied = true;
            }
            else
            {
                foreach (var item in order.OrderItems)
                {
                    var itemExcessCharge = item.GetCharge(itemExcessChargeName);
                    var itemCharge = item.GetCharge(itemChargeName);
                    
                    itemCharge.Charge += itemExcessCharge.Charge;

                    item.RemoveCharge(itemExcessChargeName);
                }

                _excessApplied = false;
            }
        }
    }
}