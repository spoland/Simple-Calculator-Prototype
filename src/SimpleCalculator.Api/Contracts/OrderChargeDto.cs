using SimpleCalculator.Domain.Models;

namespace SimpleCalculator.Api.Contracts
{
    public class OrderChargeDto
    {
        public OrderChargeDto()
        { }

        public OrderChargeDto(OrderCharge charge)
        {
            Name = charge.Name.Value;
            Amount = charge.Charge.ToString();
        }

        public string Name { get; set; } = string.Empty;

        public string Amount { get; set; } = string.Empty;
    }
}
