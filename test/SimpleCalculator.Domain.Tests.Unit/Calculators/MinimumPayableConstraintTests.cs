using FluentAssertions;
using Moq;
using SimpleCalculator.Domain.Abstractions;
using SimpleCalculator.Domain.Calculators.Constraints;
using SimpleCalculator.Domain.Constants;
using SimpleCalculator.Domain.Models;
using SimpleCalculator.Domain.ValueObjects;
using SimpleCalculator.Tests.Shared.Builders;
using System.Collections.Generic;
using Xunit;

namespace SimpleCalculator.Domain.Tests.Unit.Calculators
{
    public class MinimumPayableConstraintTests
    {
        private readonly OrderBuilder _orderBuilder;
        private readonly OrderItemBuilder _orderItemBuilder;

        public MinimumPayableConstraintTests()
        {
            _orderBuilder = new OrderBuilder();
            _orderItemBuilder = new OrderItemBuilder();
        }

        [Fact]
        public void Calculate_UnderMinimum_ShouldAddCharge()
        {
            // Arrange
            var chargeName = "VAT";
            var orderItem = _orderItemBuilder.Build();
            var order = _orderBuilder.WithOrderItems(new List<OrderItem> { orderItem }).Build();
            var calculator = Mock.Of<IChargeCalculator>();

            orderItem.AddCharge(new OrderCharge(chargeName, "EUR9.99", ChargeNames.Item));

            var sut = new MinimumPayableConstraint(calculator, chargeName, "EUR10");

            // Act
            sut.Calculate(order);

            // Assert
            order.GetChargeAmount(chargeName, order.Currency).Value.Should().Be(10);
        }

        [Fact]
        public void Calculate_UnderMinimumMultipleItems_ShouldAddCharge()
        {
            // Arrange
            var chargeName = "VAT";
            var orderItem1 = _orderItemBuilder.Build();
            var orderItem2 = _orderItemBuilder.Build();
            var order = _orderBuilder.WithOrderItems(new List<OrderItem> { orderItem1, orderItem2 }).Build();
            var calculator = Mock.Of<IChargeCalculator>();

            orderItem1.AddCharge(new OrderCharge(chargeName, "EUR4.99", ChargeNames.Item));
            orderItem1.AddCharge(new OrderCharge(chargeName, "EUR4.99", ChargeNames.Item));

            var sut = new MinimumPayableConstraint(calculator, chargeName, "EUR10");

            // Act
            sut.Calculate(order);

            // Assert
            order.GetChargeAmount(chargeName, order.Currency).Value.Should().Be(10);
            orderItem1.GetChargeAmount(chargeName, order.Currency).Value.Should().Be(5);
            orderItem2.GetChargeAmount(chargeName, order.Currency).Value.Should().Be(5);
        }
    }
}
