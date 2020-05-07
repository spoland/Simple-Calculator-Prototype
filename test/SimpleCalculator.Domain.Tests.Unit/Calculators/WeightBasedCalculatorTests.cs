using FluentAssertions;
using SimpleCalculator.Domain.Calculators;
using SimpleCalculator.Domain.Models;
using SimpleCalculator.Domain.Tests.Unit.Builders;
using SimpleCalculator.Domain.ValueObjects;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SimpleCalculator.Domain.Tests.Unit.Calculators
{
    //public class WeightBasedCalculatorTests
    //{
    //    private readonly OrderItemBuilder _orderItemBuilder;
    //    private readonly OrderBuilder _orderBuilder;

    //    public WeightBasedCalculatorTests()
    //    {
    //        _orderItemBuilder = new OrderItemBuilder();
    //        _orderBuilder = new OrderBuilder();
    //    }

    //    [Fact]
    //    public void Calculate_SingleOrderItem_ShouldCalculateCorrectAmount()
    //    {
    //        // Arrange
    //        const string chargeName = "Duty";

    //        var orderItem = _orderItemBuilder
    //            .WithQuantity(1)
    //            .WithWeight(10)
    //            .Build();

    //        var order = _orderBuilder
    //            .WithCountryIso("IE")
    //            .WithOrderItems(new List<OrderItem> { orderItem })
    //            .Build();

    //        var rate = new Percentage(10);

    //        var expectedCharge = new OrderCharge(new ChargeName(chargeName), new Price("EUR1"));

    //        var sut = new WeightBasedChargeCalculator(new ChargeName(chargeName), rate);

    //        // Act
    //        sut.Calculate(order);

    //        // Assert
    //        order.Charges.Should().ContainSingle();
    //        order.Charges.Single().Should().Be(expectedCharge);
    //    }

    //    [Fact]
    //    public void Calculate_MultipleOrderItem_ShouldCalculateCorrectAmount()
    //    {
    //        // Arrange
    //        const string chargeName = "Duty";

    //        var orderItem1 = _orderItemBuilder
    //            .WithQuantity(1)
    //            .WithWeight(10)
    //            .Build();

    //        var orderItem2 = _orderItemBuilder
    //            .WithQuantity(3)
    //            .WithWeight(15)
    //            .Build();

    //        var order = _orderBuilder
    //            .WithCountryIso("IE")
    //            .WithOrderItems(new List<OrderItem> { orderItem1, orderItem2 })
    //            .Build();

    //        var rate = new Percentage(20);

    //        var expectedCharge1 = new OrderCharge(new ChargeName(chargeName), new Price("EUR2"));
    //        var expectedCharge2 = new OrderCharge(new ChargeName(chargeName), new Price("EUR9"));

    //        var sut = new WeightBasedChargeCalculator(new ChargeName(chargeName), rate);

    //        // Act
    //        sut.Calculate(order);

    //        // Assert
    //        order.Charges.Should().HaveCount(2);
    //        order.Charges.First().Should().Be(expectedCharge1);
    //        order.Charges.Last().Should().Be(expectedCharge2);
    //    }
    //}
}