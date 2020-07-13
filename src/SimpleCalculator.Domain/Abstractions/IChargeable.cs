using SimpleCalculator.Domain.ValueObjects;
using System.Collections.Generic;

namespace SimpleCalculator.Domain.Abstractions
{
    public interface IChargeable
    {
        public Price GetChargeAmount(ChargeName chargeName, Currency currency);

        public void RemoveCharge(ChargeName chargeName);

        public IEnumerable<OrderCharge> Charges { get; }
    }
}
