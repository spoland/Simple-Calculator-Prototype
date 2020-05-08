using SimpleCalculator.Domain.Options;
using SimpleCalculator.Domain.ValueObjects;

namespace SimpleCalculator.Domain.Models
{
    public class ExcessConfiguration
    {
        public static ExcessConfiguration FromOptions(ExcessConfigurationOptions options)
        {
            return new ExcessConfiguration(options.BaseCharge, options.Amount);
        }

        public ExcessConfiguration(ChargeName baseCharge, Price amount)
        {
            BaseCharge = baseCharge;
            Amount = amount;
        }

        public ChargeName BaseCharge { get; }

        public Price Amount { get; }
    }
}
