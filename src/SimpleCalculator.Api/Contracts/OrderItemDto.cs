using SimpleCalculator.Domain.Models;

namespace SimpleCalculator.Api.Contracts
{
    public class OrderItemDto
    {
        public OrderItemDto()
        { }

        public OrderItemDto(OrderItem orderItem)
        {
            Quantity = orderItem.Quantity.Value;
            Weight = orderItem.Weight.Value;
        }

        public int Quantity { get; set; } = 0;

        public decimal Weight { get; set; } = 0;

        public decimal VatRate { get; set; } = 0;

        public decimal DutyRate { get; set; } = 0;

        public string Price { get; set; } = string.Empty;
    }
}
