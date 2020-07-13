namespace SimpleCalculator.Api.Contracts
{
    public class OrderChargeResponse
    {
        public OrderChargeResponse(string name, string amount)
        {
            Name = name;
            Amount = amount;
        }

        public string Name { get; }

        public string Amount { get; }
    }
}
