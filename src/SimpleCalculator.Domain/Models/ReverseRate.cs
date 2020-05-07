using SimpleCalculator.Domain.ValueObjects;

namespace SimpleCalculator.Domain.Models
{
    public class ReverseRate
    {
        public ReverseRate(ChargeName name, ChargeName parentChargeName, Rate rate)
        {
            Name = name;
            ParentChargeName = parentChargeName;
            Rate = rate;
        }

        /// <summary>
        /// The charge name that this reverse rate can be used to reverse out (duty, tax etc.).
        /// </summary>
        public ChargeName Name { get; }

        /// <summary>
        /// The parent charge name.
        /// If this rate has been calculated based on another rate, this helps identify the parent.
        /// For example, if vat is calculated on duty, the rate name will be VatOnDuty and the parent
        /// rate name will be Vat. This is useful when trying to group all vat related rates together
        /// during forward and/or reverse calculations.
        /// </summary>
        public ChargeName ParentChargeName { get; }

        /// <summary>
        /// The rate value that was applied.
        /// </summary>
        public Rate Rate { get; }
    }
}
