using System.Collections.Generic;

namespace SimpleCalculator.Domain.Options
{
    public class CalculatorConfigurationOptions
    {
        /// <summary>
        /// The configuration ID.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        public ExcessConfigurationOptions? Excess { get; set; } = null;

        /// <summary>
        /// A list of charges that should be used when determining what the base price to use
        /// for calculations is.
        /// </summary>
        public List<string> DeminimisBaseCharges { get; set; } = new List<string>();

        /// <summary>
        /// Contains a collection of calculator configurations.
        /// Each configuration is made up of a string key and collection of <see cref="ChargeConfigurationOptions"/>.
        /// </summary>
        public List<ChargeConfigurationOptions> ChargeConfigurations { get; set; } = new List<ChargeConfigurationOptions>();
    }
}
