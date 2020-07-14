using SimpleCalculator.Domain.Abstractions;
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
        public static ChargeName FromBaseChargeName(ChargeName chargeName, ChargeName baseChargeName)
            => new ChargeName($"{chargeName}On{baseChargeName}");

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
        public static implicit operator string(ChargeName value) => value.Value;

        public override string ToString() => Value;
    }
}
