using SimpleCalculator.Domain.Factories;
using SimpleCalculator.Domain.Models;
using SimpleCalculator.Domain.Options;
using SimpleCalculator.Domain.ValueObjects;
using System;
using System.Linq;

namespace SimpleCalculator.Api.Commands
{
    /// <summary>
    /// Obviously just for demonstration purposes :)
    /// </summary>
    public static class ForwardCalculateCommand
    {
        public static void Execute(Order order, CalculatorConfigurationOptions _options)
        {
            // Create configuration model
            var calculatorConfiguration = CalculatorConfiguration.CreateFromOptions(_options);

            // Determine deminimis base
            var deminimisBase = order.Charges.Where(chargeName => calculatorConfiguration.DeminimisBaseCharges.Contains(chargeName.Name))
                .Select(x => x.Charge.Amount).Sum();

            // Get the correct range for the base price
            var range = calculatorConfiguration.GetRangeForBasePrice(new Price(order.CurrencyIso, deminimisBase));

            // Create a forward calculator for the selected range
            var calculator = ForwardCalculatorFactory.Create(range, calculatorConfiguration.Excess);

            // Run calculator
            calculator?.Invoke(order);

            // Add a total charge for visibility
            foreach(var item in order.OrderItems)
            {
                var totalItemCharge = item.GetTotalCalculatedCharge();
                item.AddCharge(totalItemCharge);
            }
        }
    }
}
