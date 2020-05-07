using SimpleCalculator.Core.Abstractions;
using System;
using System.Collections.Generic;

namespace SimpleCalculator.Domain.ValueObjects
{
    public class CountryIso : ValueObject
    {
        public CountryIso(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"An {nameof(value)} value must be provided.", nameof(value));
            }

            Value = value;

        }

        public readonly string Value;

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        private List<string> ValidCountryIsos() => new List<string> { "IE", "US", "GB", "CH" };
    }
}