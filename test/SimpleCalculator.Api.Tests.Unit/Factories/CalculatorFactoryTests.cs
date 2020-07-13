using FluentAssertions;
using SimpleCalculator.Api.Factories;
using SimpleCalculator.Api.Options;
using SimpleCalculator.Domain.Calculators;
using SimpleCalculator.Domain.Constants;
using SimpleCalculator.Domain.Enums;
using SimpleCalculator.Domain.Factories;
using SimpleCalculator.Domain.Models;
using SimpleCalculator.Domain.Models.ChargeConfigurations;
using SimpleCalculator.Domain.ValueObjects;
using System.Collections.Generic;
using Xunit;

namespace SimpleCalculator.Api.Tests.Unit.Factories
{
    public class CalculatorFactoryTests
    {
        [Fact]
        public void Create_MultipleCalculators_ShouldCorrectlyCreateCalculator()
        {
            // Arrange
            var chargeConfiguration1 = ChargeConfigurationFactory.CreateFromOptions(new ChargeConfigurationOptions
            {
                CalculationType = CalculationType.Fixed,
                DeminimisThreshold = "EUR0",
                FixedChargeAmount = "EUR10",
                ChargeName = ChargeNames.Duty,
                Rate = 5
            });
            var chargeConfiguration2 = ChargeConfigurationFactory.CreateFromOptions(new ChargeConfigurationOptions
            {
                CalculationType = CalculationType.RateBased,
                DeminimisThreshold = "EUR0",
                BaseCharges = { "Item", ChargeNames.Duty },
                ChargeName = ChargeNames.Vat,
                Rate = 5
            });
            var chargeConfiguration3 = ChargeConfigurationFactory.CreateFromOptions(new ChargeConfigurationOptions
            {
                CalculationType = CalculationType.WeightBased,
                DeminimisThreshold = "EUR0",
                ChargeName = "Fee",
                Rate = 5
            });

            var calculationRange = new CalculationRange(new Price("EUR100"), new List<ChargeConfiguration> { chargeConfiguration1, chargeConfiguration2, chargeConfiguration3 });

            // Act
            var calculator = ForwardCalculatorFactory.Create(calculationRange).GetInvocationList();

            // Assert
            calculator.Should().HaveCount(3);
            calculator[0].Method.DeclaringType.Should().Be(typeof(FixedChargeCalculator));
            calculator[1].Method.DeclaringType.Should().Be(typeof(RateBasedChargeCalculator));
            calculator[2].Method.DeclaringType.Should().Be(typeof(WeightBasedChargeCalculator));
        }
    }
}