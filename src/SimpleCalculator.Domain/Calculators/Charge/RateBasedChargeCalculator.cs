using SimpleCalculator.Domain.Abstractions;
using SimpleCalculator.Domain.Entities;
using SimpleCalculator.Domain.Models;
using SimpleCalculator.Domain.ValueObjects;
using System;
using System.Collections.Generic;

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
                foreach (var baseChargeName in _baseCharges)
                {
                    var baseChargeAmount = item.GetTotalCharge(baseChargeName);

                    if (baseChargeAmount.ChargeAmount.Amount == 0)
                        continue;
                                        
                    var chargeAmount = baseChargeAmount.ChargeAmount * _getRate(item).AsDecimal;
                                        
                    var chargeName = ChargeName.FromBaseChargeName(_chargeName, baseChargeName);
                                        
                    item.AddCharge(new OrderCharge(chargeName, chargeAmount, _chargeName));
                }                
            }
        }
    }
}