namespace SimpleCalculator.Api.Contracts
{
    public class OrderItemRequest
    {
        public int Quantity { get; set; } = 0;

        public decimal Weight { get; set; } = 0;

        public decimal VatRate { get; set; } = 0;

        public decimal DutyRate { get; set; } = 0;

        public string Price { get; set; } = string.Empty;
    }
}
