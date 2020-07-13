using SimpleCalculator.Domain.Abstractions;
using System.Collections.Generic;
using System.Diagnostics;

namespace SimpleCalculator.Domain.ValueObjects
{
    [DebuggerDisplay("{ChargeName}{ChargeAmount}")]
    public class OrderCharge : ValueObject
    {
        /// <summary>
        /// Creates a new <see cref="OrderCharge"/> object.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="amount"></param>
        /// <param name="baseChargeName"></param>
        public OrderCharge(ChargeName name, Price amount, ChargeName baseChargeName)
        {
            ChargeName = name;
            BaseChargeName = baseChargeName;

            ChargeAmount = amount;
        }

        /// <summary>
        /// The charge name (duty, tax etc.)
        /// </summary>
        public ChargeName ChargeName { get; }

        /// <summary>
        /// The parent charge name.
        /// If this charge has been calculated based on another charge, this helps identify the parent.
        /// For example, if vat is calculated on duty, the charge name will be VatOnDuty and the parent
        /// charge name will be Vat. This is useful when trying to group all vat related rates together
        /// during forward and/or reverse calculations.
        /// </summary>
        public ChargeName BaseChargeName { get; }

        /// <summary>
        /// The charge.
        /// </summary>
        public Price ChargeAmount { get; set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return ChargeName;
            yield return ChargeAmount;
            yield return BaseChargeName;
        }
    }
}
