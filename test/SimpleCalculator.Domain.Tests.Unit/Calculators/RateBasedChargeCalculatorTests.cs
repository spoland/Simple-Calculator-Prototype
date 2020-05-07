using FluentAssertions;
using SimpleCalculator.Domain.Calculators;
using SimpleCalculator.Domain.Constants;
using SimpleCalculator.Domain.Models;
using SimpleCalculator.Domain.Tests.Unit.Builders;
using SimpleCalculator.Domain.ValueObjects;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SimpleCalculator.Domain.Tests.Unit.Calculators
{
    public class RateBasedChargeCalculatorTests
    {
        //private readonly OrderItemBuilder _orderItemBuilder;
        //private readonly OrderBuilder _orderBuilder;

        //public RateBasedChargeCalculatorTests()
        //{
        //    _orderItemBuilder = new OrderItemBuilder();
        //    _orderBuilder = new OrderBuilder();
        //}

        //[Fact]
        //public void Calculate_SingleOrderItem_ShouldCalculateCorrectAmount()
        //{
        //    // Arrange
        //    var orderItem = _orderItemBuilder
        //        .WithDutyRate(10)
        //        .WithPrice(100)
        //        .WithQuantity(1)
        //        .Build();

        //    var order = _orderBuilder
        //        .WithCountryIso("IE")
        //        .WithOrderItems(new List<OrderItem> { orderItem })
        //        .Build();

        //    var baseCharges =
        //    var expectedCharge = new OrderCharge(new ChargeName(ChargeNames.Duty), new Price("EUR10"));

        //    var sut = new RateBasedChargeCalculator(new ChargeName(ChargeNames.Duty), (oi) => oi.DutyRate);

        //    // Act
        //    sut.Calculate(order);

        //    // Assert
        //    order.Charges.Should().ContainSingle();
        //    order.Charges.Single().Should().Be(expectedCharge);
        //}

        //[Fact]
        //public void Calculate_SingleOrderItemWithKnownRate_ShouldCalculateCorrectAmount()
        //{
        //    // Arrange
        //    var orderItem = _orderItemBuilder
        //        .WithPrice(100)
        //        .WithQuantity(1)
        //        .Build();

        //    var order = _orderBuilder
        //        .WithCountryIso("IE")
        //        .WithOrderItems(new List<OrderItem> { orderItem })
        //        .Build();

        //    var expectedCharge = new OrderCharge(new ChargeName(ChargeNames.Duty), new Price("EUR10"));

        //    var sut = new RateBasedChargeCalculator(new ChargeName(ChargeNames.Duty), (oi) => new Percentage(10));

        //    // Act
        //    sut.Calculate(order);

        //    // Assert
        //    order.Charges.Should().ContainSingle();
        //    order.Charges.Single().Should().Be(expectedCharge);
        //}

        //[Fact]
        //public void Calculate_MultipleOrderItem_ShouldCalculateCorrectAmounts()
        //{
        //    // Arrange
        //    var orderItem1 = _orderItemBuilder
        //        .WithDutyRate(10)
        //        .WithPrice(100)
        //        .Build();

        //    var orderItem2 = _orderItemBuilder
        //        .WithDutyRate(15)
        //        .WithPrice(100)
        //        .Build();

        //    var order = _orderBuilder
        //        .WithCountryIso("IE")
        //        .WithOrderItems(new List<OrderItem> { orderItem1, orderItem2 })
        //        .Build();

        //    var expectedCharge1 = new OrderCharge(new ChargeName(ChargeNames.Duty), new Price("EUR10"));
        //    var expectedCharge2 = new OrderCharge(new ChargeName(ChargeNames.Duty), new Price("EUR15"));

        //    var sut = new RateBasedChargeCalculator(new ChargeName(ChargeNames.Duty), (oi) => oi.DutyRate);

        //    // Act
        //    sut.Calculate(order);

        //    // Assert
        //    order.Charges.Should().HaveCount(2);
        //    order.Charges.First().Should().Be(expectedCharge1);
        //    order.Charges.Last().Should().Be(expectedCharge2);
        //}
    }
}