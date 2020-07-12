using SimpleCalculator.Domain.Abstractions;
using SimpleCalculator.Domain.Exceptions;
using System;
using System.Collections.Generic;

namespace SimpleCalculator.Domain.ValueObjects
{
    public class Money : ValueObject, IComparable<Money>
    {
        public Money(Currency currency, decimal amount)
        {
            Currency = currency;
            Value = amount;
        }

        public Money(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("A valid money value must be provided.");

            try
            {
                var currency = new Currency(value.Substring(0, 3));
                var amount = decimal.Parse(value.Substring(3));

                Currency = currency;
                Value = amount;
            }
            catch (Exception)
            {
                throw new InvalidMoneyValueException($"{value} is not a valid money string.");
            }
        }

        public readonly Currency Currency;

        public decimal Value { get; set; }

        public static bool operator <(Money money1, Money money2)
        {
            if (money1.Currency != money2.Currency)
                throw new CurrencyMismatchException($"Unable to compare {money1} to {money2} as they are of different currencies.");

            return money1.Value < money2.Value;
        }

        public static bool operator >(Money money1, Money money2)
        {
            if (money1.Currency != money2.Currency)
                throw new CurrencyMismatchException($"Unable to compare {money1} to {money2} as they are of different currencies.");

            return money1.Value > money2.Value;
        }

        public static bool operator >=(Money money1, Money money2)
        {
            if (money1.Currency != money2.Currency)
                throw new CurrencyMismatchException($"Unable to compare {money1} to {money2} as they are of different currencies.");

            return money1.Value >= money2.Value;
        }

        public static bool operator <=(Money money1, Money money2)
        {
            if (money1.Currency != money2.Currency)
                throw new CurrencyMismatchException($"Unable to compare {money1} to {money2} as they are of different currencies.");

            return money1.Value <= money2.Value;
        }

        public static Money operator +(Money summand1, Money summand2)
        {
            if (summand1.Currency != summand2.Currency)
                throw new CurrencyMismatchException($"Unable to add {summand1} to {summand2} as they are of different currencies.");

            return new Money(summand1.Currency, summand1.Value + summand2.Value);
        }

        public static Money operator -(Money minuend, Money subtrahend)
        {
            if (minuend.Currency != subtrahend.Currency)
                throw new CurrencyMismatchException($"Unable to subtract {subtrahend} from {minuend} as they are of different currencies.");

            return new Money(minuend.Currency, minuend.Value - subtrahend.Value);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Currency;
            yield return Value;
        }

        public override string ToString() => $"{Currency.Value}{Value}";

        public int CompareTo(Money other)
        {
            if (Value < other.Value)
                return -1;

            if (Value > other.Value)
                return 1;

            return 0;
        }
    }
}
