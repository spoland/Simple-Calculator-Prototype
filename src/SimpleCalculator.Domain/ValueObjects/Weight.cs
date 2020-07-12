using System;

namespace SimpleCalculator.Domain.ValueObjects
{
    public class Weight
    {
        public static Weight InKilogrames(decimal value)
        {
            return new Weight(value);
        }

        private Weight(decimal value)
        {
            if (value <= 0)
                throw new ArgumentException("Weight value cannot be 0.");

            Value = value;
        }

        /// <summary>
        /// Returns the weight value in Kilograms
        /// </summary>
        public decimal Value { get; }

        /// <summary>
        /// Returns the weight value in pounds
        /// </summary>
        public decimal ValueInPounds => Value / 2.20462262m;
    }
}
