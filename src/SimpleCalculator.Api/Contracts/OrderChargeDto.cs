namespace SimpleCalculator.Api.Contracts
{
    public class OrderChargeDto
    {
        public OrderChargeDto(string name, string amount)
        {
            Name = name;
            Amount = amount;
        }

        public string Name { get; }

        public string Amount { get; }
    }
}
