using SimpleCalculator.Domain.Abstractions;
using SimpleCalculator.Domain.Entities;
using SimpleCalculator.Domain.Models;
using SimpleCalculator.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleCalculator.Domain.Calculators
{
    public class RateBasedChargeCalculator : IChargeCalculator
    {
        private readonly ChargeName _chargeName;
        private readonly Func<OrderItem, Rate> _getRate;
        private readonly IEnumerable<ChargeName> _baseCharges;

        public RateBasedChargeCalculator(
            ChargeName chargeName,
            Func<OrderItem, Rate> getRate,
            IEnumerable<ChargeName> baseCharges)
        {
            _getRate = getRate;
            _baseCharges = baseCharges;
            _chargeName = chargeName;
        }

        public void Calculate(Order order)
        {
            foreach (var item in order.OrderItems)
            {
                // For each base charge that this charge should be calculated on top of (item, duty, vat etc.)
                foreach (var baseChargeName in _baseCharges)
                {
                    // All of the charges that we need to calculate on top of
                    // e.g if applying on top of item and vat, and vat was applied on item, the charges we need to calculate on are:
                    // item, vatOnItem
                    var baseCharges = item.Charges.Where(c => c.BaseChargeName == baseChargeName);

                    foreach(var charge in baseCharges.ToList())
                    {
                        if (charge.ChargeAmount.Value == 0)
                            continue;

                        var chargeAmount = charge.ChargeAmount * _getRate(item).AsDecimal;

                        var chargeName = ChargeName.FromBaseChargeName(_chargeName, charge.ChargeName);

                        item.AddCharge(new OrderCharge(chargeName, chargeAmount, _chargeName));
                    }
                }
            }
        }
    }
}