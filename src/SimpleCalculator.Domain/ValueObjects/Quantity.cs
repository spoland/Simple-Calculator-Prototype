using SimpleCalculator.Domain.Abstractions;
using System;
using System.Collections.Generic;

namespace SimpleCalculator.Domain.ValueObjects
{
    public class Quantity : ValueObject
    {
        public Quantity(int value)
        {
            if (value < 0)
                throw new ArgumentException("A quantity cannot be less than zero.");

            Value = value;
        }

        public int Value { get; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
