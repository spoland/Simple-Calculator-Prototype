using FluentAssertions;
using SimpleCalculator.Domain.Calculators;
using SimpleCalculator.Domain.Models;
using SimpleCalculator.Tests.Shared.Builders;
using System.Collections.Generic;
using Xunit;

namespace CalculatorPrototype.Domain.Tests.Unit.Services.ChargeCalculators
{
    public class WeightBasedCalculatorTests
    {
        private readonly OrderBuilder _orderBuilder;
        private readonly OrderItemBuilder _orderItemBuilder;
        private const string ChargeName = "WeightBasedCharge";

        public WeightBasedCalculatorTests()
        {
            _orderBuilder = new OrderBuilder();
            _orderItemBuilder = new OrderItemBuilder();
        }

        [Fact]
        public void Calculate_SingleOrderItem_ShouldAddCorrectCharge()
        {
            // Arrange
            var orderItem = _orderItemBuilder
                .WithWeight(10)
                .WithDutyRate(10)
                .WithQuantity(1)
                .Build();

            var order = _orderBuilder.WithOrderItems(new List<OrderItem> { orderItem }).Build();

            var sut = new WeightBasedChargeCalculator(ChargeName, oi => oi.DutyRate);

            // Act
            sut.Calculate(order);

            // Assert
            orderItem.Charges.Should()
                .HaveCount(2).And
                .ContainSingle(c => c.ChargeName.Value == ChargeName && c.ChargeAmount.Value == 1);
        }

        [Fact]
        public void Calculate_MultipleOrderItems_ShouldAddCorrectCharge()
        {
            // Arrange
            var orderItem1 = _orderItemBuilder
                .WithWeight(10)
                .WithDutyRate(10)
                .WithQuantity(1)
                .Build();

            var orderItem2 = _orderItemBuilder
                .WithWeight(10)
                .WithDutyRate(20)
                .WithQuantity(2)
                .Build();

            var order = _orderBuilder.WithOrderItems(new List<OrderItem> { orderItem1, orderItem2 }).Build();

            var sut = new WeightBasedChargeCalculator(ChargeName, oi => oi.DutyRate);

            // Act
            sut.Calculate(order);

            // Assert
            orderItem1.Charges.Should()
                .HaveCount(2).And
                .ContainSingle(c => c.ChargeName.Value == ChargeName && c.ChargeAmount.Value == 1);
            orderItem2.Charges.Should()
                .HaveCount(2).And
                .ContainSingle(c => c.ChargeName.Value == ChargeName && c.ChargeAmount.Value == 4);
        }
    }
}
