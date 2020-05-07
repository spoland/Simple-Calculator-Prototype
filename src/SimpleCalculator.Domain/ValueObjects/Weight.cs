using System;

namespace SimpleCalculator.Domain.ValueObjects
{
    public class Weight
    {
        public Weight(decimal valueInKilograms)
        {
            if (valueInKilograms <= 0)
                throw new ArgumentException("Weight value cannot be 0.");

            Value = valueInKilograms;
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
