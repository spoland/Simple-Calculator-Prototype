using SimpleCalculator.Domain.Abstractions;
using SimpleCalculator.Domain.Constants;
using SimpleCalculator.Domain.Entities;
using SimpleCalculator.Domain.ValueObjects;
using System.Linq;

namespace SimpleCalculator.Domain.Calculators.Charge
{
    public class ReverseItemPriceCalculator : IChargeCalculator
    {
        public void Calculate(Order order)
        {
            foreach (var item in order.OrderItems)
            {
                // Sum up all known charges
                var knownCharges = item.Charges.Where(x => x.ChargeName.Value != ChargeNames.InputItem).Select(x => x.ChargeAmount).Sum();

                // Remove known charges from inclusive price
                var partiallyReversedItemPrice = item.GetCharge(ChargeNames.InputItem).ChargeAmount - knownCharges;

                // Reverse out the remaining charges using the calculated reverse rates
                var itemPrice = partiallyReversedItemPrice.Value / (1 + item.ReverseRates.Sum(x => x.Rate.AsDecimal));

                // Add the item charge
                item.AddCharge(new OrderCharge(ChargeNames.Item, new Price(partiallyReversedItemPrice.Currency, itemPrice), ChargeNames.Item));

                // Remove leftover charges
                item.ResetKnownCharges();
            }            
        }
    }
}