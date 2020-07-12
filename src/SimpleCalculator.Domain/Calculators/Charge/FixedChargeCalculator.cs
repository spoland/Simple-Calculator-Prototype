﻿using SimpleCalculator.Domain.Abstractions;
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
                var chargeAmount = _amount * item.CostRelativeToOrderTotal;
                item.AddCharge(new OrderCharge(_chargeName, chargeAmount, _chargeName));
            }
        }
    }
}
