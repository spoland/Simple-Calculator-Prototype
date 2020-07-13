using SimpleCalculator.Domain.Abstractions;
using SimpleCalculator.Domain.Entities;
using SimpleCalculator.Domain.Models;
using SimpleCalculator.Domain.ValueObjects;
using System;

namespace SimpleCalculator.Domain.Calculators
{
    public class WeightBasedChargeCalculator : IChargeCalculator
    {
        private readonly ChargeName _chargeName;
        private readonly Func<OrderItem, Rate> _getRate;

        public WeightBasedChargeCalculator(ChargeName chargeName, Func<OrderItem, Rate> getRate)
        {
            _chargeName = chargeName;
            _getRate = getRate;
        }

        public void Calculate(Order order)
        {
            foreach (var item in order.OrderItems)
            {
                var chargeAmount = item.Weight.Value * _getRate(item).AsDecimal * item.Quantity.Value;
                item.AddCharge(new OrderCharge(_chargeName, new Price(order.Currency, chargeAmount), _chargeName));
            }
        }
    }
}
