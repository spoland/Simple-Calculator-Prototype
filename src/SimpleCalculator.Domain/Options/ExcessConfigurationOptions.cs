namespace SimpleCalculator.Domain.Options
{
    public class ExcessConfigurationOptions
    {
        /// <summary>
        /// The excess amount that is not chargeable.
        /// </summary>
        public string Amount { get; set; } = string.Empty;

        public string BaseCharge { get; set; } = string.Empty;
    }
}
