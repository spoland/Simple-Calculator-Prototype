using SimpleCalculator.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace SimpleCalculator.Api.Contracts
{
    public class OrderResponse
    {
        public OrderResponse(Order order)
        {
            DeclarationCountry = order.Country.Value;
            Currency = order.Currency.Value;
            OrderItems = order.OrderItems.Select(x => new OrderItemResponse(x.Charges));
        }

        public string DeclarationCountry { get; }

        public string Currency { get; }

        public IEnumerable<OrderItemResponse> OrderItems { get; }
    }
}
