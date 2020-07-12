using SimpleCalculator.Domain.ValueObjects;

namespace SimpleCalculator.Api.Contracts
{
    public class OrderChargeDto
    {
        public OrderChargeDto()
        { }

        public OrderChargeDto(OrderCharge charge)
        {
            Name = charge.ChargeName.Value;
            Amount = charge.ChargeAmount.ToString();
        }

        public string Name { get; set; } = string.Empty;

        public string Amount { get; set; } = string.Empty;
    }
}
