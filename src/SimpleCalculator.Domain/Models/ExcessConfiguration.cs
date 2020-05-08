using SimpleCalculator.Domain.Options;
using SimpleCalculator.Domain.ValueObjects;

namespace SimpleCalculator.Domain.Models
{
    public class ExcessConfiguration
    {
        public static ExcessConfiguration FromOptions(ExcessConfigurationOptions options)
        {
            return new ExcessConfiguration(options.BaseCharge, options.Amount, options.InputCharge ?? null as ChargeName);
        }

        /// <summary>
        /// Creates a new <see cref="ExcessConfiguration"/>.
        /// </summary>
        /// <param name="baseCharge">The base charge that the excess should be applied to when calculating forward.</param>
        /// <param name="amount">The excess amount that should be applied when calculating.</param>
        /// <param name="inputCharge">The input charge that should be used in reverse calculations.</param>
        public ExcessConfiguration(ChargeName baseCharge, Price amount, ChargeName? inputCharge)
        {
            BaseCharge = baseCharge;
            Amount = amount;
            InputCharge = inputCharge;
        }

        /// <summary>
        /// The inclusive charge that includes the charge that had an excess applied.
        /// For example, if an excess was applied to the item charge which then had 
        /// duties and charges applied to it, we'll need to apply the excess to the
        /// InputItem charge when reversing. This is only required when reverse
        /// calculating.
        /// </summary>
        public ChargeName? InputCharge { get; }

        /// <summary>
        /// The charge that the excess should be applied to.
        /// </summary>
        public ChargeName BaseCharge { get; }

        /// <summary>
        /// The excess amount that should be excluded from duties/taxes.
        /// </summary>
        public Price Amount { get; }
    }
}
