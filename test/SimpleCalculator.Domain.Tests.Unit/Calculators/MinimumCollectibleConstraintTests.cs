using FluentAssertions;
using Moq;
using SimpleCalculator.Domain.Abstractions;
using SimpleCalculator.Domain.Calculators.Constraints;
using SimpleCalculator.Domain.Models;
using SimpleCalculator.Domain.ValueObjects;
using SimpleCalculator.Tests.Shared.Builders;
using System.Collections.Generic;
using Xunit;

namespace SimpleCalculator.Domain.Tests.Unit.Calculators
{
    public class MinimumCollectibleConstraintTests
    {
        private readonly OrderBuilder _orderBuilder;
        private readonly OrderItemBuilder _orderItemBuilder;

        public MinimumCollectibleConstraintTests()
        {
            _orderBuilder = new OrderBuilder();
            _orderItemBuilder = new OrderItemBuilder();
        }

        [Fact]
        public void Calculate_UnderMinimum_ShouldRemoveCharge()
        {
            // Arrange
            var chargeName = "VAT";
            var orderItem = _orderItemBuilder.Build();
            var order = _orderBuilder.WithOrderItems(new List<OrderItem> { orderItem }).Build();
            var calculator = Mock.Of<IChargeCalculator>();

            orderItem.AddCharge(new OrderCharge(chargeName, "EUR9.99"));

            var sut = new MinimumCollectibleConstraint(calculator, chargeName, "EUR10");

            // Act
            sut.Calculate(order);

            // Assert
            order.GetCharge(chargeName, order.Currency).ChargeAmount.Value.Should().Be(0);
        }

        [Fact]
        public void Calculate_UnderMinimumMultipleItems_ShouldRemoveCharge()
        {
            // Arrange
            var chargeName = "VAT";
            var orderItem1 = _orderItemBuilder.Build();
            var orderItem2 = _orderItemBuilder.Build();
            var order = _orderBuilder.WithOrderItems(new List<OrderItem> { orderItem1, orderItem2 }).Build();
            var calculator = Mock.Of<IChargeCalculator>();

            orderItem1.AddCharge(new OrderCharge(chargeName, "EUR5"));
            orderItem2.AddCharge(new OrderCharge(chargeName, "EUR4.99"));

            var sut = new MinimumCollectibleConstraint(calculator, chargeName, "EUR10");

            // Act
            sut.Calculate(order);

            // Assert
            order.GetCharge(chargeName, order.Currency).ChargeAmount.Value.Should().Be(0);
        }

        [Fact]
        public void Calculate_AboveMinimum_ShouldNotRemoveCharge()
        {
            // Arrange
            var chargeName = "VAT";
            var orderItem = _orderItemBuilder.Build();
            var order = _orderBuilder.WithOrderItems(new List<OrderItem> { orderItem }).Build();
            var calculator = Mock.Of<IChargeCalculator>();

            orderItem.AddCharge(new OrderCharge(chargeName, "EUR10"));

            var sut = new MinimumCollectibleConstraint(calculator, chargeName, "EUR10");

            // Act
            sut.Calculate(order);

            // Assert
            order.GetCharge(chargeName, order.Currency).ChargeAmount.Value.Should().Be(10);
        }

        [Fact]
        public void Calculate_AboveMinimumMultipleItems_ShouldNotRemoveCharge()
        {
            // Arrange
            var chargeName = "VAT";
            var orderItem1 = _orderItemBuilder.Build();
            var orderItem2 = _orderItemBuilder.Build();
            var order = _orderBuilder.WithOrderItems(new List<OrderItem> { orderItem1, orderItem2 }).Build();
            var calculator = Mock.Of<IChargeCalculator>();

            orderItem1.AddCharge(new OrderCharge(chargeName, "EUR10"));
            orderItem2.AddCharge(new OrderCharge(chargeName, "EUR10"));

            var sut = new MinimumCollectibleConstraint(calculator, chargeName, "EUR10");

            // Act
            sut.Calculate(order);

            // Assert
            order.GetCharge(chargeName, order.Currency).ChargeAmount.Value.Should().Be(20);
        }
    }
}
