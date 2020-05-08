namespace SimpleCalculator.Domain.Options
{
    public class ExcessConfigurationOptions
    {
        /// <summary>
        /// The excess amount that is not chargeable.
        /// </summary>
        public string Amount { get; set; } = string.Empty;

        /// <summary>
        /// The charge that the excess should be applied to in forward calculations.
        /// </summary>
        public string BaseCharge { get; set; } = string.Empty;

        /// <summary>
        /// The charge that the excess should be applied to in reverse calculations.
        /// </summary>
        public string? InputCharge { get; set; } = null;
    }
}
