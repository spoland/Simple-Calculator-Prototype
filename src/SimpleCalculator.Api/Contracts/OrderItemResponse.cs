using SimpleCalculator.Domain.ValueObjects;
using System.Collections.Generic;
using System.Linq;

namespace SimpleCalculator.Api.Contracts
{
    public class OrderItemResponse
    {
        public OrderItemResponse(IEnumerable<OrderCharge> charges)
        {
            Charges = charges
                .Where(x => !x.InputCharge)
                .Select(x => new OrderChargeResponse(x.ChargeName, x.ChargeAmount))
                .OrderBy(x => x.Name);
        }

        public IEnumerable<OrderChargeResponse> Charges { get; }
    }
}
