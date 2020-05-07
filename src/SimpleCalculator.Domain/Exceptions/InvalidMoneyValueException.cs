using System;
using System.Runtime.Serialization;

namespace SimpleCalculator.Domain.Exceptions
{
    public class InvalidMoneyValueException : Exception
    {
        public InvalidMoneyValueException()
        {
        }

        public InvalidMoneyValueException(string message) : base(message)
        {
        }

        public InvalidMoneyValueException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidMoneyValueException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
