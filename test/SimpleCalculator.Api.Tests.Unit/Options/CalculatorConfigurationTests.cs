using FluentAssertions;
using SimpleCalculator.Api.Options;
using SimpleCalculator.Domain.Enums;
using SimpleCalculator.Domain.Exceptions;
using SimpleCalculator.Domain.Models;
using SimpleCalculator.Domain.Models.ChargeConfigurations;
using SimpleCalculator.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SimpleCalculator.Domain.Tests.Unit
{
    public class CalculatorConfigurationTests
    {
        [Fact]
        public void Create_EmptyConfigurationCollection_ShouldThrowArgumentException()
        {
            // Arrange
            var deminimisBaseCharges = new List<ChargeName> { "Item" };
            var chargeConfigurations = new List<ChargeConfiguration>();

            // Act
            Action create = () => new CalculatorConfiguration(chargeConfigurations, deminimisBaseCharges);

            // Assert
            create.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Create_DuplicateConfigurationsInRange_ShouldThrow()
        {
            // Arrange
            var deminimisBaseCharges = new List<ChargeName> { "Item" };
            var chargeConfigurations = new List<ChargeConfiguration> 
            {
                new RateBasedChargeConfiguration("Duty", "EUR0", new List<ChargeName> { "Item" }),
                new RateBasedChargeConfiguration("Duty", "EUR0", new List<ChargeName> { "Item" })
            };

            // Act
            Action create = () => new CalculatorConfiguration(chargeConfigurations, deminimisBaseCharges);

            // Assert
            create.Should().Throw<Exception>();
        }

        [Fact]
        public void Create_MultipleThreshold_ShouldCorrectlyCreateRanges()
        {
            // Arrange
            var deminimisBaseCharges = new List<ChargeName> { "Item" };
            var chargeConfigurations = new List<ChargeConfiguration>
            {
                new RateBasedChargeConfiguration("Duty", "EUR0", new List<ChargeName> { "Item" }),
                new RateBasedChargeConfiguration("Vat", "EUR100", new List<ChargeName> { "Item", "Duty" }),
                new RateBasedChargeConfiguration("Fee", "EUR200", new List<ChargeName> { "Item", "Duty", "Vat" }),
                new FixedRateChargeConfiguration("FixedFee", "EUR300", "EUR100")
            };

            // Act
            var calculationRanges = new CalculatorConfiguration(chargeConfigurations, deminimisBaseCharges)
                .CalculationRanges
                .ToList();

            // Assert
            calculationRanges.Should().HaveCount(4);

            calculationRanges[0].DeminimisThreshold.ToString().Should().Be("EUR0");
            calculationRanges[0].ChargeConfigurations.Should().HaveCount(1);

            calculationRanges[1].DeminimisThreshold.ToString().Should().Be("EUR100");
            calculationRanges[1].ChargeConfigurations.Should().HaveCount(2);

            calculationRanges[2].DeminimisThreshold.ToString().Should().Be("EUR200");
            calculationRanges[2].ChargeConfigurations.Should().HaveCount(3);

            calculationRanges[3].DeminimisThreshold.ToString().Should().Be("EUR300");
            calculationRanges[3].ChargeConfigurations.Should().HaveCount(4);
        }

        [Fact]
        public void FromChargeConfigurations_ComplexBaseCharges_ShouldCorrectlyOrderConfigurations()
        {
            // Arrange
            var deminimisBaseCharges = new List<ChargeName> { "Item" };
            var chargeConfigurations = new List<ChargeConfiguration>
            {
                new RateBasedChargeConfiguration("FinalInterest", "EUR0", new List<ChargeName> { "Item", "Duty", "Vat", "Fee", "FixedDuty", "FixedInterest", "AnotherInterest" }),
                new RateBasedChargeConfiguration("Fee", "EUR0", new List<ChargeName> { "Item", "Duty", "Vat" }),
                new FixedRateChargeConfiguration("FixedInterest", "EUR0", "EUR100"),
                new RateBasedChargeConfiguration("Vat", "EUR0", new List<ChargeName> { "Item", "Duty" }),
                new RateBasedChargeConfiguration("AnotherInterest", "EUR0", new List<ChargeName> { "Item", "Fee" }),
                new FixedRateChargeConfiguration("FixedDuty", "EUR0", "EUR100"),
                new RateBasedChargeConfiguration("Duty", "EUR0", new List<ChargeName> { "Item" })
            };

            // Act
            var configuration = new CalculatorConfiguration(chargeConfigurations, deminimisBaseCharges);
            var calculationRange = configuration.CalculationRanges.Single().ChargeConfigurations.ToList();

            // Assert
            calculationRange[0].ChargeName.Value.Should().Be("FixedDuty");
            calculationRange[1].ChargeName.Value.Should().Be("FixedInterest");
            calculationRange[2].ChargeName.Value.Should().Be("Duty");
            calculationRange[3].ChargeName.Value.Should().Be("Vat");
            calculationRange[4].ChargeName.Value.Should().Be("Fee");
            calculationRange[5].ChargeName.Value.Should().Be("AnotherInterest");
            calculationRange[6].ChargeName.Value.Should().Be("FinalInterest");
        }

        [Fact]
        public void FromChargeConfigurations_RegionChargeOverride_ShouldCorrectlyCreateRegions()
        {
            // Arrange
            var deminimisBaseCharges = new List<ChargeName> { "Item" };
            var chargeConfigurations = new List<ChargeConfiguration>
            {
                new FixedRateChargeConfiguration("Duty", "EUR100", "EUR10"),
                new FixedRateChargeConfiguration("Duty", "EUR200", "EUR20"),
                new FixedRateChargeConfiguration("Duty", "EUR300", "EUR30")
            };

            // Act
            var calculationRanges = new CalculatorConfiguration(chargeConfigurations, deminimisBaseCharges).CalculationRanges.ToList();

            // Assert
            calculationRanges[0].ChargeConfigurations.Single().As<FixedRateChargeConfiguration>().FixedChargeAmount.ToString().Should().Be("EUR10");
            calculationRanges[1].ChargeConfigurations.Single().As<FixedRateChargeConfiguration>().FixedChargeAmount.ToString().Should().Be("EUR20");
            calculationRanges[2].ChargeConfigurations.Single().As<FixedRateChargeConfiguration>().FixedChargeAmount.ToString().Should().Be("EUR30");
        }

        [Fact]
        public void FromChargeConfigurations_CyclicBaseCharges_ShouldThrow()
        {
            // Arrange
            var deminimisBaseCharges = new List<ChargeName> { "Item" };
            var chargeConfigurations = new List<ChargeConfiguration>
            {
                new RateBasedChargeConfiguration("Duty", "EUR0", new List<ChargeName> { "Vat" }),
                new RateBasedChargeConfiguration("Vat", "EUR0", new List<ChargeName> { "Duty" })
            };

            // Act
            Action create = () => new CalculatorConfiguration(chargeConfigurations, deminimisBaseCharges);

            // Assert
            create.Should().Throw<InvalidChargeConfigurationException>()
                .WithMessage("Cyclic dependencies exist between charge configurations.");
        }

        [Fact]
        public void FromChargeConfigurations_ConfigurationsWithBaseCharges_ShouldSetCorrectBaseCharges()
        {
            // Arrange
            var deminimisBaseCharges = new List<ChargeName> { "Item" };
            var chargeConfigurations = new List<ChargeConfiguration>
            {
                new RateBasedChargeConfiguration("Duty", "EUR0", new List<ChargeName> { "Item" }),
                new RateBasedChargeConfiguration("Vat", "EUR0", new List<ChargeName> { "Item", "Duty" }),
                new RateBasedChargeConfiguration("Fee", "EUR0", new List<ChargeName> { "Item", "Duty", "Vat" })
            };            

            // Act
            var configurations = new CalculatorConfiguration(chargeConfigurations, deminimisBaseCharges)
                .CalculationRanges
                .Single()
                .ChargeConfigurations;

            // Assert
            configurations.Single(x => x.ChargeName.Value == "Duty")
                .As<RateBasedChargeConfiguration>().BaseCharges.Should()
                .BeEmpty();

            configurations.Single(x => x.ChargeName.Value == "Vat")
                .As<RateBasedChargeConfiguration>().BaseCharges.Should()
                .OnlyContain(x => x.ChargeName.Value == "Duty");

            configurations.Single(x => x.ChargeName.Value == "Fee")
                .As<RateBasedChargeConfiguration>().BaseCharges.Should()
                .ContainSingle(x => x.ChargeName.Value == "Duty").And
                .ContainSingle(x => x.ChargeName.Value == "Vat");
        }

        [Theory]
        [InlineData("EUR50", "EUR0")]
        [InlineData("EUR150", "EUR100")]
        [InlineData("EUR250", "EUR200")]
        public void GetCalculationRange_MultipleRanges_ShouldReturnCorrectRegions(string price, string expectedThreshold)
        {
            // Arrange
            var deminimisBaseCharges = new List<ChargeName> { "Item" };
            var chargeConfigurations = new List<ChargeConfiguration>
            {
                new FixedRateChargeConfiguration("Duty", "EUR0", "EUR10"),
                new FixedRateChargeConfiguration("Duty", "EUR100", "EUR20"),
                new FixedRateChargeConfiguration("Duty", "EUR200", "EUR30")
            };

            var calculationRanges = new CalculatorConfiguration(chargeConfigurations, deminimisBaseCharges);

            // Act
            var range = calculationRanges.GetRangeForBasePrice(new Price(price));

            // Assert
            range.DeminimisThreshold.Should().Be(new Price(expectedThreshold));
        }
    }
}