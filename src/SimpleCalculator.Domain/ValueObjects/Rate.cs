using SimpleCalculator.Domain.Abstractions;
using System;
using System.Collections.Generic;

namespace SimpleCalculator.Domain.ValueObjects
{
    public class Rate : ValueObject
    {
        public Rate(decimal value)
        {
            if (!(value >= 0 && value <= 100))
                throw new ArgumentException("Invalid percentage value, must be between 0-100");

            Value = value;
        }

        public decimal Value;

        public decimal AsDecimal => Value / 100;

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public static implicit operator Rate(decimal value) => new Rate(value);
    }
}
