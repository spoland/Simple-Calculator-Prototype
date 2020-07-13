using FluentAssertions;
using SimpleCalculator.Domain.Calculators;
using SimpleCalculator.Domain.Models;
using SimpleCalculator.Domain.ValueObjects;
using SimpleCalculator.Tests.Shared.Builders;
using SimpleCalculator.Tests.Shared.Mocks;
using System.Collections.Generic;
using Xunit;

namespace SimpleCalculator.Domain.Tests.Unit.Calculators
{
    public class FixedChargeCalculatorTests
    {
        private const string ChargeName = "FixedCharge";
        private readonly OrderBuilder _orderBuilder;
        private readonly OrderItemBuilder _orderItemBuilder;

        public FixedChargeCalculatorTests()
        {
            _orderBuilder = new OrderBuilder();
            _orderItemBuilder = new OrderItemBuilder();
        }

        [Fact]
        public void Calculate_SingleOrderItem_ShouldAddCorrectCharge()
        {
            // Arrange
            var orderItem = _orderItemBuilder.Build();
            var order = _orderBuilder.WithOrderItems(new List<OrderItem> { orderItem }).Build();

            var sut = new FixedChargeCalculator(ChargeName, new Price(CurrencyFakes.EUR, 10));

            // Act
            sut.Calculate(order);

            // Assert
            orderItem.Charges.Should()
                .HaveCount(3).And
                .ContainSingle(c => c.ChargeName.Value == ChargeName && c.ChargeAmount.Value == 10);
        }

        [Fact]
        public void Calculate_MultipleOrderItems_ShouldAddCorrectCharge()
        {
            // Arrange
            var orderItem1 = _orderItemBuilder.WithPrice("EUR", 25m).Build();
            var orderItem2 = _orderItemBuilder.WithPrice("EUR", 75m).Build();
            var order = _orderBuilder.WithOrderItems(new List<OrderItem> { orderItem1, orderItem2 }).Build();

            var sut = new FixedChargeCalculator(ChargeName, new Price(CurrencyFakes.EUR, 20));

            // Act
            sut.Calculate(order);

            // Assert
            orderItem1.Charges.Should()
                .HaveCount(3).And
                .ContainSingle(c => c.ChargeName.Value == ChargeName && c.ChargeAmount.Value == 5);
            orderItem2.Charges.Should()
                .HaveCount(3).And
                .ContainSingle(c => c.ChargeName.Value == ChargeName && c.ChargeAmount.Value == 15);
        }
    }
}
