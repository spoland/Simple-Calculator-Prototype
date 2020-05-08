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
        private ChargeName _excessChargeName;

        public ExcessCalculator(ExcessConfiguration excessConfiguration)
        {
            _excessAmount = excessConfiguration.Amount;
            _excessChargeName = excessConfiguration.BaseCharge;
        }

        public void Calculate(Order order)
        {
            ChargeName excessChargeName = ChargeNames.Excess;
            ChargeName itemExcessChargeName = ChargeName.FromBaseChargeName(excessChargeName, _excessChargeName);

            if (!_excessApplied)
            {
                var totalItemCharge = order.GetCharge(_excessChargeName);

                if (totalItemCharge.Charge > _excessAmount)
                {
                    foreach(var item in order.OrderItems)
                    {
                        var itemExcessAmount = _excessAmount * order.RelativeItemValue(item).AsDecimal;
                        var itemExcessCharge = new OrderCharge(itemExcessChargeName, itemExcessAmount, new ChargeName("excess"));

                        var itemCharge = item.GetCharge(_excessChargeName);
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
                    var itemCharge = item.GetCharge(_excessChargeName);
                    
                    itemCharge.Charge += itemExcessCharge.Charge;

                    item.RemoveCharge(itemExcessChargeName);
                }

                _excessApplied = false;
            }
        }
    }
}