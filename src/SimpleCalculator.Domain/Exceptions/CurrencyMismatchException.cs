using System;
using System.Runtime.Serialization;

namespace SimpleCalculator.Domain.Exceptions
{
    public class CurrencyMismatchException : Exception
    {
        public CurrencyMismatchException()
        {
        }

        public CurrencyMismatchException(string message) : base(message)
        {
        }

        public CurrencyMismatchException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CurrencyMismatchException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
