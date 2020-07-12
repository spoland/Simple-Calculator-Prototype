using SimpleCalculator.Domain.Constants;
using SimpleCalculator.Domain.Entities;
using SimpleCalculator.Domain.Factories;
using SimpleCalculator.Domain.Models;
using SimpleCalculator.Domain.Options;
using SimpleCalculator.Domain.ValueObjects;
using System.Linq;

namespace SimpleCalculator.Api.Commands
{
    public class ReverseCalculateCommand
    {
        public static void Execute(Order order, CalculatorConfigurationOptions options)
        {
            // Create configuration model
            var calculatorConfiguration = CalculatorConfiguration.CreateFromOptions(options);

            // Loop through each range until correct one found
            foreach (var range in calculatorConfiguration.CalculationRanges.Reverse())
            {
                // Create reverse calc using range
                var reverseCalculator = ReverseCalculatorFactory.Create(range);

                // Run reverse calc
                reverseCalculator?.Invoke(order);

                // Create forward calc
                var forwardCalculator = ForwardCalculatorFactory.Create(range, calculatorConfiguration.Excess);

                // Run forward calc
                forwardCalculator?.Invoke(order);

                // Get and compare charges
                var inputCharge = order.GetCharge(ChargeNames.InputItem);
                var totalCharge = order.GetTotalCalculatedCharge();

                // Determine deminimis base
                var deminimisBase = order.Charges.Where(chargeName => calculatorConfiguration.DeminimisBaseCharges.Contains(chargeName.ChargeName))
                    .Select(x => x.ChargeAmount.Amount).Sum();
                
                if (inputCharge.ChargeAmount == totalCharge.ChargeAmount && new Price(order.CurrencyIso, deminimisBase) >= range.DeminimisThreshold)
                    break;

                // if charges don't match reset and run again
                order.ResetCalculationProperties();
            }
        }
    }
}