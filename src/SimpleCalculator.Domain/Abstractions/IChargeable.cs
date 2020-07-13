using SimpleCalculator.Domain.ValueObjects;
using System.Collections.Generic;

namespace SimpleCalculator.Domain.Abstractions
{
    public interface IChargeable
    {
        public OrderCharge GetCharge(ChargeName chargeName);

        public IEnumerable<OrderCharge> Charges { get; }
    }
}
