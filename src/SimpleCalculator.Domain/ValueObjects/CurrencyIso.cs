using SimpleCalculator.Core.Abstractions;
using System;
using System.Collections.Generic;

namespace SimpleCalculator.Domain.ValueObjects
{
    public class CurrencyIso : ValueObject
    {
        public CurrencyIso(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"An {nameof(value)} value must be provided.", nameof(value));
            }

            if (ValidCurrencyIsos().Contains(value))
                Value = value;
            else
                throw new ArgumentException($"{value} is not a known currency code.");
        }

        public readonly string Value;

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        private List<string> ValidCurrencyIsos() => new List<string> { "EUR", "USD" };
    }
}