using SimpleCalculator.Core.Abstractions;
using SimpleCalculator.Domain.Constants;
using System;
using System.Collections.Generic;

namespace SimpleCalculator.Domain.ValueObjects
{
    public class ChargeName : ValueObject
    {
        /// <summary>
        /// Generates a charge name from another charge name. For example, VatOnFee.
        /// </summary>
        /// <param name="chargeName"></param>
        /// <param name="baseChargeName"></param>
        /// <returns></returns>
        public static ChargeName FromParentChargeName(ChargeName chargeName, ChargeName baseChargeName)
        {
            if (chargeName.Value == ChargeNames.Item)
                return new ChargeName(chargeName.Value);

            return new ChargeName($"{chargeName}On{baseChargeName}");
        }

        public ChargeName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"A valid {nameof(value)} must be provided.", nameof(value));
            }

            Value = value;
        }

        public string Value { get; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public static implicit operator ChargeName(string value) => new ChargeName(value);

        public override string ToString() => Value;
    }
}
