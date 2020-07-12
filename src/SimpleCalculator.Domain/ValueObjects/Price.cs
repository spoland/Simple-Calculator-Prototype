using SimpleCalculator.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleCalculator.Domain.ValueObjects
{
    public class Price : Money
    {
        public Price(Currency currency, decimal amount) : base(currency, amount)
        {
            if (amount < 0)
                throw new ArgumentException("A price cannot be negative.");
        }

        public Price(string value) : base(value)
        {
            var amount = decimal.Parse(value.Substring(3));

            if (amount < 0)
                throw new ArgumentException("A price cannot be negative.");
        }

        public static implicit operator Price(string value) => new Price(value);

        public static Price operator *(Price price, decimal multiplier)
        {
            return new Price(price.Currency, price.Value * multiplier);
        }

        public static Price operator +(Price summand1, Price summand2)
        {
            if (summand1.Currency != summand2.Currency)
                throw new CurrencyMismatchException($"Unable to add {summand1} to {summand2} as they are of different currencies.");

            return new Price(summand1.Currency, summand1.Value + summand2.Value);
        }

        public static Price operator -(Price minuend, Price subtrahend)
        {
            if (minuend.Currency != subtrahend.Currency)
                throw new CurrencyMismatchException($"Unable to subtract {subtrahend} from {minuend} as they are of different currencies.");

            return new Price(minuend.Currency, minuend.Value - subtrahend.Value);
        }
    }

    public static class PriceExtensions 
    {
        public static Price Sum(this IEnumerable<Price> source)
        {
            if (source.Any())
                return source.Aggregate((x, y) => x + y);

            return new Price("EUR0");
        }
    }
}