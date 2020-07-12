using FluentAssertions;
using SimpleCalculator.Domain.Calculators;
using SimpleCalculator.Domain.Constants;
using SimpleCalculator.Domain.Models;
using SimpleCalculator.Domain.ValueObjects;
using SimpleCalculator.Tests.Shared.Builders;
using SimpleCalculator.Tests.Shared.Mocks;
using System.Collections.Generic;
using Xunit;

namespace CalculatorPrototype.Domain.Tests.Unit.Services.ChargeCalculators
{
    public class ReverseRateCalculatorTests
    {
        private readonly OrderBuilder _orderBuilder;
        private readonly OrderItemBuilder _orderItemBuilder;

        public ReverseRateCalculatorTests()
        {
            _orderBuilder = new OrderBuilder();
            _orderItemBuilder = new OrderItemBuilder();
        }

        [Fact]
        public void Calculate_WhenBaseChargeAlreadyCalculated_ShouldCalculateForwardAndAddCorrectCharge()
        {
            // Arrange
            var baseCharges = new List<ChargeName> { ChargeNames.Duty };

            var orderItem = _orderItemBuilder
                .WithVatRate(10)
                .Build();

            orderItem.AddCharge(new OrderCharge("DutyOnItem", new Price(CurrencyFakes.EUR, 100), ChargeNames.Duty));

            var order = _orderBuilder
                .WithOrderItems(new List<OrderItem> { orderItem })
                .Build();

            var sut = new ReverseRateCalculator(ChargeNames.Vat, oi => oi.VatRate, baseCharges);

            // Act
            sut.Calculate(order);

            // Assert
            orderItem.ReverseRates.Should().BeEmpty();
            orderItem.Charges.Should().ContainSingle(c => c.ChargeName.Value == "VatOnDuty" && c.ChargeAmount.Amount == 10);
        }

        [Fact]
        public void Calculate_WhenBaseChargeIsNotACalculatedCharge_ShouldAddRateToReverseRatesCollection()
        {
            // Arrange
            var baseCharges = new List<ChargeName> { ChargeNames.Item };

            var orderItem = _orderItemBuilder
                .WithVatRate(10)
                .Build();

            var order = _orderBuilder
                .WithOrderItems(new List<OrderItem> { orderItem })
                .Build();

            var sut = new ReverseRateCalculator(ChargeNames.Vat, oi => oi.VatRate, baseCharges);

            // Act
            sut.Calculate(order);

            // Assert
            orderItem.ReverseRates.Should()
                .ContainSingle(rr => rr.Rate == orderItem.VatRate && rr.ChargeName.Value == "VatOnItem" && rr.BaseChargeName.Value == ChargeNames.Vat);
        }

        [Fact]
        public void Calculate_WhenBaseChargeIncludesACalculatedCharge_ShouldAddMultipleRatesToReverseRatesCollection()
        {
            // Arrange
            var dutyRate = new Rate(5);
            var baseCharges = new List<ChargeName> { ChargeNames.Item, ChargeNames.Duty };

            var orderItem = _orderItemBuilder
                .WithVatRate(10)
                .Build();

            orderItem.AddReverseRate(new ReverseRate("DutyOnItem", ChargeNames.Duty, dutyRate));

            var order = _orderBuilder
                .WithOrderItems(new List<OrderItem> { orderItem })
                .Build();

            var sut = new ReverseRateCalculator(ChargeNames.Vat, oi => oi.VatRate, baseCharges);

            // Act
            sut.Calculate(order);

            // Assert
            orderItem.ReverseRates.Should()
                .HaveCount(3).And
                .ContainSingle(rr => rr.Rate == orderItem.VatRate && rr.ChargeName.Value == "VatOnItem" && rr.BaseChargeName.Value == ChargeNames.Vat).And
                .ContainSingle(rr => rr.Rate == dutyRate && rr.ChargeName.Value == "DutyOnItem" && rr.BaseChargeName.Value == ChargeNames.Duty).And
                .ContainSingle(rr => rr.Rate == new Rate(0.5m) && rr.ChargeName.Value == "VatOnDutyOnItem" && rr.BaseChargeName.Value == ChargeNames.Vat);
        }
    }
}
