using FluentAssertions;
using SimpleCalculator.Domain.Enums;
using SimpleCalculator.Domain.Exceptions;
using SimpleCalculator.Domain.Models;
using SimpleCalculator.Domain.Models.ChargeConfigurations;
using SimpleCalculator.Domain.Options;
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
            var calculatorConfiguration = new CalculatorConfigurationOptions
            {
                Id = "IE",
                DeminimisBaseCharges = { "Item" },
                ChargeConfigurations = new List<ChargeConfigurationOptions>()
            };

            // Act
            Action create = () => new CalculatorConfiguration(calculatorConfiguration);

            // Assert
            create.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Create_DuplicateConfigurationsInRange_ShouldThrow()
        {
            // Arrange
            var calculatorConfiguration = new CalculatorConfigurationOptions
            {
                Id = "IE",
                DeminimisBaseCharges = { "Item" },
                ChargeConfigurations = new List<ChargeConfigurationOptions>
                {
                    new ChargeConfigurationOptions
                    {
                        Name = "Duty",
                        CalculationType = CalculationType.RateBased,
                        BaseCharges = new List<string> { "Item" },
                        DeminimisThreshold = "EUR0"
                    },
                    new ChargeConfigurationOptions
                    {
                        Name = "Duty",
                        CalculationType = CalculationType.RateBased,
                        BaseCharges = new List<string> { "Item" },
                        DeminimisThreshold = "EUR0"
                    }
                }
            };

            // Act
            Action create = () => new CalculatorConfiguration(calculatorConfiguration);

            // Assert
            create.Should().Throw<Exception>();
        }

        [Fact]
        public void Create_MultipleThreshold_ShouldCorrectlyCreateRanges()
        {
            // Arrange
            var calculatorConfiguration = new CalculatorConfigurationOptions
            {
                Id = "IE",
                DeminimisBaseCharges = { "Item" },
                ChargeConfigurations = new List<ChargeConfigurationOptions>
                {
                    new ChargeConfigurationOptions
                    {
                        Name = "Duty",
                        CalculationType = CalculationType.RateBased,
                        BaseCharges = new List<string> { "Item" },
                        DeminimisThreshold = "EUR0"
                    },
                    new ChargeConfigurationOptions
                    {
                        Name = "Vat",
                        CalculationType = CalculationType.RateBased,
                        BaseCharges = new List<string> { "Item", "Duty" },
                        DeminimisThreshold = "EUR100"
                    },
                    new ChargeConfigurationOptions
                    {
                        Name = "Fee",
                        CalculationType = CalculationType.RateBased,
                        BaseCharges = new List<string> { "Item", "Duty", "Vat" },
                        DeminimisThreshold = "EUR200"
                    },
                    new ChargeConfigurationOptions
                    {
                        Name = "Interest",
                        CalculationType = CalculationType.Fixed,
                        BaseCharges = new List<string>(),
                        FixedChargeAmount = "EUR100",
                        DeminimisThreshold = "EUR300"
                    }
                }
            };

            // Act
            var calculationRanges = new CalculatorConfiguration(calculatorConfiguration)
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
            var configurationOptions = new CalculatorConfigurationOptions
            {
                Id = "IE",
                DeminimisBaseCharges = { "Item" },
                ChargeConfigurations = new List<ChargeConfigurationOptions>
                {
                    new ChargeConfigurationOptions
                    {
                        Name = "FinalInterest",
                        CalculationType = CalculationType.RateBased,
                        BaseCharges = new List<string> { "Item", "Duty", "Vat", "Fee", "FixedDuty", "FixedInterest", "AnotherInterest" },
                        DeminimisThreshold = "EUR0"
                    },
                    new ChargeConfigurationOptions
                    {
                        Name = "Fee",
                        CalculationType = CalculationType.RateBased,
                        BaseCharges = new List<string> { "Item", "Duty", "Vat" },
                        DeminimisThreshold = "EUR0"
                    },
                    new ChargeConfigurationOptions
                    {
                        Name = "FixedInterest",
                        CalculationType = CalculationType.Fixed,
                        BaseCharges = new List<string>(),
                        DeminimisThreshold = "EUR0",
                        FixedChargeAmount = "EUR100"
                    },
                    new ChargeConfigurationOptions
                    {
                        Name = "Vat",
                        CalculationType = CalculationType.RateBased,
                        BaseCharges = new List<string> { "Item", "Duty" },
                        DeminimisThreshold = "EUR0"
                    },
                    new ChargeConfigurationOptions
                    {
                        Name = "AnotherInterest",
                        CalculationType = CalculationType.RateBased,
                        BaseCharges = new List<string> { "Item", "Fee" },
                        DeminimisThreshold = "EUR0"
                    },
                    new ChargeConfigurationOptions
                    {
                        Name = "FixedDuty",
                        CalculationType = CalculationType.Fixed,
                        BaseCharges = new List<string>(),
                        FixedChargeAmount = "EUR100",
                        DeminimisThreshold = "EUR0"
                    },
                    new ChargeConfigurationOptions
                    {
                        Name = "Duty",
                        CalculationType = CalculationType.RateBased,
                        BaseCharges = new List<string> { "Item" },
                        DeminimisThreshold = "EUR0"
                    }
                }
            };

            // Act
            var configuration = new CalculatorConfiguration(configurationOptions);
            var calculationRange = configuration.CalculationRanges.Single().ChargeConfigurations.ToList();

            // Assert
            calculationRange[0].Name.Value.Should().Be("FixedDuty");
            calculationRange[1].Name.Value.Should().Be("FixedInterest");
            calculationRange[2].Name.Value.Should().Be("Duty");
            calculationRange[3].Name.Value.Should().Be("Vat");
            calculationRange[4].Name.Value.Should().Be("Fee");
            calculationRange[5].Name.Value.Should().Be("AnotherInterest");
            calculationRange[6].Name.Value.Should().Be("FinalInterest");
        }

        [Fact]
        public void FromChargeConfigurations_RegionChargeOverride_ShouldCorrectlyCreateRegions()
        {
            // Arrange
            var configurationOptions = new CalculatorConfigurationOptions
            {
                Id = "IE",
                DeminimisBaseCharges = { "Item" },
                ChargeConfigurations = new List<ChargeConfigurationOptions>
                {
                    new ChargeConfigurationOptions
                    {
                        Name = "Duty",
                        CalculationType = CalculationType.Fixed,
                        DeminimisThreshold = "EUR100",
                        FixedChargeAmount = "EUR10"
                    },
                    new ChargeConfigurationOptions
                    {
                        Name = "Duty",
                        CalculationType = CalculationType.Fixed,
                        DeminimisThreshold = "EUR200",
                        FixedChargeAmount = "EUR20"
                    },
                    new ChargeConfigurationOptions
                    {
                        Name = "Duty",
                        CalculationType = CalculationType.Fixed,
                        DeminimisThreshold = "EUR300",
                        FixedChargeAmount = "EUR30"
                    }
                }
            };

            // Act
            var calculationRanges = new CalculatorConfiguration(configurationOptions).CalculationRanges.ToList();

            // Assert
            calculationRanges[0].ChargeConfigurations.Single().As<FixedRateChargeConfiguration>().FixedChargeAmount.ToString().Should().Be("EUR10");
            calculationRanges[1].ChargeConfigurations.Single().As<FixedRateChargeConfiguration>().FixedChargeAmount.ToString().Should().Be("EUR20");
            calculationRanges[2].ChargeConfigurations.Single().As<FixedRateChargeConfiguration>().FixedChargeAmount.ToString().Should().Be("EUR30");
        }

        [Fact]
        public void FromChargeConfigurations_CyclicBaseCharges_ShouldThrow()
        {
            // Arrange
            var configurationOptions = new CalculatorConfigurationOptions
            {
                Id = "IE",
                DeminimisBaseCharges = { "Item" },
                ChargeConfigurations = new List<ChargeConfigurationOptions>
                {
                    new ChargeConfigurationOptions
                    {
                        Name = "Duty",
                        CalculationType = CalculationType.RateBased,
                        BaseCharges = new List<string> { "Vat" },
                        FixedChargeAmount = "EUR30",
                        DeminimisThreshold = "EUR0"
                    },
                    new ChargeConfigurationOptions
                    {
                        Name = "Vat",
                        CalculationType = CalculationType.RateBased,
                        BaseCharges = new List<string> { "Duty" },
                        FixedChargeAmount = "EUR30",
                        DeminimisThreshold = "EUR0"
                    }
                }
            };

            // Act
            Action create = () => new CalculatorConfiguration(configurationOptions);

            // Assert
            create.Should().Throw<InvalidChargeConfigurationException>()
                .WithMessage("Cyclic dependencies exist between charge configurations.");
        }

        [Fact]
        public void FromChargeConfigurations_ConfigurationsWithBaseCharges_ShouldSetCorrectBaseCharges()
        {
            // Arrange
            var configurationOptions = new CalculatorConfigurationOptions
            {
                Id = "IE",
                DeminimisBaseCharges = { "Item" },
                ChargeConfigurations = new List<ChargeConfigurationOptions>
                {
                    new ChargeConfigurationOptions
                    {
                        Name = "Duty",
                        CalculationType = CalculationType.RateBased,
                        BaseCharges = new List<string> { "Item" },
                        DeminimisThreshold = "EUR0"
                    },
                    new ChargeConfigurationOptions
                    {
                        Name = "Vat",
                        CalculationType = CalculationType.RateBased,
                        BaseCharges = new List<string> { "Item", "Duty" },
                        DeminimisThreshold = "EUR0"
                    },
                    new ChargeConfigurationOptions
                    {
                        Name = "Fee",
                        CalculationType = CalculationType.RateBased,
                        BaseCharges = new List<string> { "Item", "Duty", "Vat" },
                        DeminimisThreshold = "EUR0"
                    }
                }
            };

            // Act
            var configurations = new CalculatorConfiguration(configurationOptions)
                .CalculationRanges
                .Single()
                .ChargeConfigurations;

            // Assert
            configurations.Single(x => x.Name.Value == "Duty")
                .As<RateBasedChargeConfiguration>().BaseCharges.Should().BeEmpty();

            configurations.Single(x => x.Name.Value == "Vat")
                .As<RateBasedChargeConfiguration>().BaseCharges.Should().OnlyContain(x => x.Name.Value == "Duty");

            configurations.Single(x => x.Name.Value == "Fee")
                .As<RateBasedChargeConfiguration>().BaseCharges.Should().ContainSingle(x => x.Name.Value == "Duty").And
                .ContainSingle(x => x.Name.Value == "Vat");
        }

        [Theory]
        [InlineData("EUR50", "EUR0")]
        [InlineData("EUR150", "EUR100")]
        [InlineData("EUR250", "EUR200")]
        public void GetCalculationRange_MultipleRanges_ShouldReturnCorrectRegions(string price, string expectedThreshold)
        {
            // Arrange
            var configurationOptions = new CalculatorConfigurationOptions
            {
                Id = "IE",
                DeminimisBaseCharges = { "Item" },
                ChargeConfigurations = new List<ChargeConfigurationOptions>
                {
                    new ChargeConfigurationOptions
                    {
                        Name = "Duty",
                        CalculationType = CalculationType.Fixed,
                        DeminimisThreshold = "EUR0",
                        FixedChargeAmount = "EUR10"
                    },
                    new ChargeConfigurationOptions
                    {
                        Name = "Duty",
                        CalculationType = CalculationType.Fixed,
                        DeminimisThreshold = "EUR100",
                        FixedChargeAmount = "EUR20"
                    },
                    new ChargeConfigurationOptions
                    {
                        Name = "Duty",
                        CalculationType = CalculationType.Fixed,
                        DeminimisThreshold = "EUR200",
                        FixedChargeAmount = "EUR30"
                    }
                }
            };

            var calculationRanges = new CalculatorConfiguration(configurationOptions);

            // Act
            var range = calculationRanges.GetRangeForBasePrice(new Price(price));

            // Assert
            range.DeminimisThreshold.Should().Be(new Price(expectedThreshold));
        }
    }
}