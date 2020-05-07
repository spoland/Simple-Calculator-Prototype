using System;
using System.Runtime.Serialization;

namespace SimpleCalculator.Domain.Exceptions
{
    public class InvalidChargeConfigurationException : Exception
    {
        public InvalidChargeConfigurationException()
        {
        }

        public InvalidChargeConfigurationException(string message) : base(message)
        {
        }

        public InvalidChargeConfigurationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidChargeConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
