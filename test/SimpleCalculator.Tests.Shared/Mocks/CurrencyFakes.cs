using SimpleCalculator.Domain.ValueObjects;

namespace SimpleCalculator.Tests.Shared.Mocks
{
    public static class CurrencyFakes
    {
        public static Currency EUR => new Currency("EUR");
        public static Currency GBP => new Currency("GBP");
        public static Currency USD => new Currency("USD");
        public static Currency AUD => new Currency("AUD");
    }
}
