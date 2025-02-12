﻿using SimpleCalculator.Domain.Abstractions;
using SimpleCalculator.Domain.Entities;
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

            var appliedCharge = order.GetChargeAmount(_chargeName, order.Currency);

            if (appliedCharge < _minimumPayable)
            {
                order.RemoveCharge(_chargeName);

                foreach(var item in order.OrderItems)
                {
                    var minimumItemCharge = _minimumPayable * order.RelativeOrderItemValue(item);
                    item.AddCharge(new OrderCharge(_chargeName, minimumItemCharge, _chargeName));
                }
            }
        }
    }
}