using SimpleCalculator.Core.Abstractions;
using SimpleCalculator.Domain.Exceptions;
using System;
using System.Collections.Generic;

namespace SimpleCalculator.Domain.ValueObjects
{
    public class Money : ValueObject, IComparable<Money>
    {
        public Money(CurrencyIso currency, decimal amount)
        {
            Currency = currency;
            Amount = amount;
        }

        public Money(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("A valid money value must be provided.");

            try
            {
                var currency = new CurrencyIso(value.Substring(0, 3));
                var amount = decimal.Parse(value.Substring(3));

                Currency = currency;
                Amount = amount;
            }
            catch (Exception)
            {
                throw new InvalidMoneyValueException($"{value} is not a valid money string.");
            }
        }

        public readonly CurrencyIso Currency;

        public decimal Amount { get; set; }

        public static bool operator <(Money money1, Money money2)
        {
            if (money1.Currency != money2.Currency)
                throw new CurrencyMismatchException($"Unable to compare {money1} to {money2} as they are of different currencies.");

            return money1.Amount < money2.Amount;
        }

        public static bool operator >(Money money1, Money money2)
        {
            if (money1.Currency != money2.Currency)
                throw new CurrencyMismatchException($"Unable to compare {money1} to {money2} as they are of different currencies.");

            return money1.Amount > money2.Amount;
        }

        public static bool operator >=(Money money1, Money money2)
        {
            if (money1.Currency != money2.Currency)
                throw new CurrencyMismatchException($"Unable to compare {money1} to {money2} as they are of different currencies.");

            return money1.Amount >= money2.Amount;
        }

        public static bool operator <=(Money money1, Money money2)
        {
            if (money1.Currency != money2.Currency)
                throw new CurrencyMismatchException($"Unable to compare {money1} to {money2} as they are of different currencies.");

            return money1.Amount <= money2.Amount;
        }

        public static Money operator +(Money summand1, Money summand2)
        {
            if (summand1.Currency != summand2.Currency)
                throw new CurrencyMismatchException($"Unable to add {summand1} to {summand2} as they are of different currencies.");

            return new Money(summand1.Currency, summand1.Amount + summand2.Amount);
        }

        public static Money operator -(Money minuend, Money subtrahend)
        {
            if (minuend.Currency != subtrahend.Currency)
                throw new CurrencyMismatchException($"Unable to subtract {subtrahend} from {minuend} as they are of different currencies.");

            return new Money(minuend.Currency, minuend.Amount - subtrahend.Amount);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Currency;
            yield return Amount;
        }

        public override string ToString() => $"{Currency.Value}{Amount}";

        public int CompareTo(Money other)
        {
            if (Amount < other.Amount)
                return -1;

            if (Amount > other.Amount)
                return 1;

            return 0;
        }
    }
}
