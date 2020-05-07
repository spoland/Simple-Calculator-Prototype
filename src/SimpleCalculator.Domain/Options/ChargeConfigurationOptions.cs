using SimpleCalculator.Domain.Enums;
using System.Collections.Generic;

namespace SimpleCalculator.Domain.Options
{
    public class ChargeConfigurationOptions
    {
        public string Name { get; set; } = string.Empty;

        public CalculationType? CalculationType { get; set; }

        public string DeminimisThreshold { get; set; } = string.Empty;

        public string? FixedChargeAmount { get; set; } = string.Empty;

        public string? MinimumPayable { get; set; } = string.Empty;

        public string? MinimumCollectible { get; set; } = string.Empty;

        public decimal? Rate { get; set; }

        public List<string> BaseCharges { get; set; } = new List<string>();
    }
}