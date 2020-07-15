using SimpleCalculator.Domain.Abstractions;
using SimpleCalculator.Domain.Models;
using System.Collections.Generic;

namespace SimpleCalculator.Domain.ValueObjects
{
    public class GreyZone : ValueObject
    {
        public GreyZone(Price from, Price to, CalculationRange calculationRange)
        {
            From = from;
            To = to;
            CalculationRange = calculationRange;
        }

        public Price From { get; }

        public Price To { get; }

        public CalculationRange CalculationRange { get; }

        public bool IsInGreyZone(Price price) => price >= From && price <= To;

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return From;
            yield return To;
            yield return CalculationRange;
        }
    }
}
