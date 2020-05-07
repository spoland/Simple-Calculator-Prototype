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
    public class FixedChargeCalculatorTests
    {
        //private readonly OrderItemBuilder _orderItemBuilder;
        //private readonly OrderBuilder _orderBuilder;

        //public FixedChargeCalculatorTests()
        //{
        //    _orderItemBuilder = new OrderItemBuilder();
        //    _orderBuilder = new OrderBuilder();
        //}

        //[Fact]
        //public void Calculate_SingleOrderItem_ShouldCalculateCorrectAmount()
        //{
        //    // Arrange
        //    const string chargeName = "Duty";

        //    var orderItem = _orderItemBuilder
        //        .WithPrice(100)
        //        .WithQuantity(1)
        //        .Build();

        //    var order = _orderBuilder
        //        .WithCountryIso("IE")
        //        .WithOrderItems(new List<OrderItem> { orderItem })
        //        .Build();

        //    var expectedCharge = new OrderCharge(new ChargeName(chargeName), new Price("EUR10"));

        //    var sut = new FixedChargeCalculator(new ChargeName(chargeName), new Price("EUR10"));

        //    // Act
        //    sut.Calculate(order);

        //    // Assert
        //    order.Charges.Should().ContainSingle();
        //    order.Charges.Single().Should().Be(expectedCharge);
        //}

        //[Fact]
        //public void Calculate_MultipleOrderItems_ShouldCalculateCorrectAmount()
        //{
        //    // Arrange
        //    const string chargeName = "Duty";

        //    var orderItem1 = _orderItemBuilder
        //        .WithQuantity(1)
        //        .WithPrice(25)
        //        .Build();

        //    var orderItem2 = _orderItemBuilder
        //        .WithQuantity(2)
        //        .WithPrice(75)
        //        .Build();

        //    var order = _orderBuilder
        //        .WithCountryIso("IE")
        //        .WithOrderItems(new List<OrderItem> { orderItem1, orderItem2 })
        //        .Build();

        //    var expectedCharge1 = new OrderCharge(new ChargeName(chargeName), new Price("EUR25"));
        //    var expectedCharge2 = new OrderCharge(new ChargeName(chargeName), new Price("EUR75"));

        //    var sut = new FixedChargeCalculator(new ChargeName(chargeName), new Price("EUR100"));

        //    // Act
        //    sut.Calculate(order);

        //    // Assert
        //    order.Charges.Should().HaveCount(2);
        //    order.Charges.First().Should().Be(expectedCharge1);
        //    order.Charges.Last().Should().Be(expectedCharge2);
        //}
    }
}
