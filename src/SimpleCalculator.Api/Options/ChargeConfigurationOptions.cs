using SimpleCalculator.Domain.Enums;
using System.Collections.Generic;

namespace SimpleCalculator.Api.Options
{
    public class ChargeConfigurationOptions
    {
        public string ChargeName { get; set; } = string.Empty;

        public CalculationType? CalculationType { get; set; }

        public string DeminimisThreshold { get; set; } = string.Empty;

        public string? FixedChargeAmount { get; set; } = default;

        public string? MinimumPayable { get; set; } = default;

        public string? MinimumCollectible { get; set; } = default;

        public decimal? Rate { get; set; } = default;

        public List<string> BaseCharges { get; set; } = new List<string>();
    }
}