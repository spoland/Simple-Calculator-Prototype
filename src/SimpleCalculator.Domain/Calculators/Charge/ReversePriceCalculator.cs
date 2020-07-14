using SimpleCalculator.Domain.Abstractions;
using SimpleCalculator.Domain.Constants;
using SimpleCalculator.Domain.Entities;
using SimpleCalculator.Domain.ValueObjects;
using System.Linq;

namespace SimpleCalculator.Domain.Calculators.Charge
{
    public class ReversePriceCalculator : IChargeCalculator
    {
        private readonly ChargeName _chargeName;
        private readonly ChargeName _inputChargeName;

        public ReversePriceCalculator(
            ChargeName chargeName,
            ChargeName inputChargeName)
        {
            _chargeName = chargeName;
            _inputChargeName = inputChargeName;
        }

        public void Calculate(Order order)
        {
            foreach (var item in order.OrderItems)
            {
                // Sum up all known charges
                var knownCharges = item.Charges
                    .Where(x => !x.IsInputCharge)
                    .Where(x => x.ChargeName != ChargeNames.Item && x.ChargeName != ChargeNames.Delivery)
                    .Select(x => x.ChargeAmount).Sum(order.Currency);

                // Remove known charges from inclusive price
                var partiallyReversedPrice = item.GetChargeAmount(_inputChargeName, order.Currency) - knownCharges;

                // Reverse out the remaining charges using the calculated reverse rates
                var applicableRates = item.ReverseRates.Where(x => x.ChargeName.Value.Contains(_chargeName));
                var itemPrice = partiallyReversedPrice.Value / (1 + applicableRates.Sum(x => x.Rate.AsDecimal));

                // Add the item charge
                item.AddCharge(new OrderCharge(_chargeName, new Price(partiallyReversedPrice.Currency, itemPrice), _chargeName));
            }
        }
    }
}