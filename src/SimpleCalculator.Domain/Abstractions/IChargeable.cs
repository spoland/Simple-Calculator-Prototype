using SimpleCalculator.Domain.ValueObjects;
using System.Collections.Generic;

namespace SimpleCalculator.Domain.Abstractions
{
    public interface IChargeable
    {
        public OrderCharge GetCharge(ChargeName chargeName, Currency currency);

        public void RemoveCharge(ChargeName chargeName);

        public IEnumerable<OrderCharge> Charges { get; }
    }
}
