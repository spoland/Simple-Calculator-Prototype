using System.Collections.Generic;
using System.Linq;

namespace SimpleCalculator.Api.Contracts
{
    public class OrderRequest
    {
        public string DeclarationCountry { get; set; } = string.Empty;

        public string Currency { get; set; } = string.Empty;

        public string DeliveryPrice { get; set; } = string.Empty;

        public IEnumerable<OrderItemRequest> OrderItems { get; set; } = Enumerable.Empty<OrderItemRequest>();
    }
}
