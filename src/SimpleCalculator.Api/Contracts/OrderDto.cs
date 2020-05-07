using SimpleCalculator.Domain.Models;
using System.Collections.Generic;
using System.Linq;

namespace SimpleCalculator.Api.Contracts
{
    public class OrderDto
    {
        public OrderDto()
        { }

        public OrderDto(Order order)
        {
            Id = order.Id.Value;
            OrderItems = order.OrderItems.Select(orderItem => new OrderItemDto(orderItem));
            CountryIso = order.CountryIso.Value;
        }

        public string Id { get; set; } = string.Empty;

        public string CountryIso { get; set; } = string.Empty;

        public string CurrencyIso { get; set; } = string.Empty;

        public string? DeliveryPrice { get; set; } = string.Empty;

        public IEnumerable<OrderItemDto> OrderItems { get; set; } = Enumerable.Empty<OrderItemDto>();
    }
}
