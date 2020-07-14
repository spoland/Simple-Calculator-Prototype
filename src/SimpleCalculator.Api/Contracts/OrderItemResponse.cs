using SimpleCalculator.Domain.ValueObjects;
using System.Collections.Generic;
using System.Linq;

namespace SimpleCalculator.Api.Contracts
{
    public class OrderItemResponse
    {
        public OrderItemResponse(IEnumerable<OrderCharge> charges)
        {
            Charges = charges.Select(x => new OrderChargeResponse(x.ChargeName, x.ChargeAmount));
        }

        public IEnumerable<OrderChargeResponse> Charges { get; }
    }
}
