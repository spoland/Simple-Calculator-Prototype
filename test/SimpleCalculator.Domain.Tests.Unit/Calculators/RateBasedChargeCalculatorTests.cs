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
    public class RateBasedChargeCalculatorTests
    {
        private readonly OrderBuilder _orderBuilder;
        private readonly OrderItemBuilder _orderItemBuilder;

        public RateBasedChargeCalculatorTests()
        {
            _orderBuilder = new OrderBuilder();
            _orderItemBuilder = new OrderItemBuilder();
        }

        [Fact]
        public void Calculate_SingleOrderItem_ShouldAddCorrectCharges()
        {
            // Arrange
            var baseCharges = new List<ChargeName>()
            {
                "Item", "Duty", "Fee"
            };

            var orderItem = _orderItemBuilder
                .WithVatRate(10)
                .WithQuantity(1)
                .Build();

            orderItem.AddCharge(new OrderCharge(ChargeNames.Item, new Price(CurrencyFakes.EUR, 100)));
            orderItem.AddCharge(new OrderCharge(ChargeNames.Duty, new Price(CurrencyFakes.EUR, 10), ChargeNames.Duty));
            orderItem.AddCharge(new OrderCharge("FeeOnDuty", new Price(CurrencyFakes.EUR, 5), ChargeNames.Fee));

            var order = _orderBuilder.WithOrderItems(new List<OrderItem> { orderItem }).Build();

            var sut = new RateBasedChargeCalculator(ChargeNames.Vat, oi => oi.VatRate, baseCharges);

            // Act
            sut.Calculate(order);

            // Assert
            orderItem.Charges.Should()
                .HaveCount(7).And
                .ContainSingle(x => x.ChargeName.Value == "VatOnItem" && x.ChargeAmount.Value == 10).And
                .ContainSingle(x => x.ChargeName.Value == "VatOnDuty" && x.ChargeAmount.Value == 1).And
                .ContainSingle(x => x.ChargeName.Value == "VatOnFee" && x.ChargeAmount.Value == 0.5m);
        }

        [Fact]
        public void Calculate_MultipleOrderItems_ShouldAddCorrectCharges()
        {
            // Arrange
            var baseCharges = new List<ChargeName>()
            {
                "Item", "Duty", "Fee"
            };

            var orderItem1 = _orderItemBuilder
                .WithVatRate(10)
                .WithQuantity(1)
                .Build();

            var orderItem2 = _orderItemBuilder
                .WithVatRate(5)
                .WithQuantity(1)
                .Build();

            orderItem1.AddCharge(new OrderCharge(ChargeNames.Item, new Price(CurrencyFakes.EUR, 100)));
            orderItem1.AddCharge(new OrderCharge("Duty", new Price(CurrencyFakes.EUR, 10), ChargeNames.Duty));
            orderItem1.AddCharge(new OrderCharge("FeeOnDuty", new Price(CurrencyFakes.EUR, 5), ChargeNames.Fee));
            orderItem2.AddCharge(new OrderCharge(ChargeNames.Item, new Price(CurrencyFakes.EUR, 100)));
            orderItem2.AddCharge(new OrderCharge("Duty", new Price(CurrencyFakes.EUR, 10), ChargeNames.Duty));
            orderItem2.AddCharge(new OrderCharge("FeeOnDuty", new Price(CurrencyFakes.EUR, 5), ChargeNames.Fee));

            var order = _orderBuilder.WithOrderItems(new List<OrderItem> { orderItem1, orderItem2 }).Build();

            var sut = new RateBasedChargeCalculator(ChargeNames.Vat, oi => oi.VatRate, baseCharges);

            // Act
            sut.Calculate(order);

            // Assert
            orderItem1.Charges.Should()
                .HaveCount(7).And
                .ContainSingle(x => x.ChargeName.Value == "VatOnItem" && x.ChargeAmount.Value == 10).And
                .ContainSingle(x => x.ChargeName.Value == "VatOnDuty" && x.ChargeAmount.Value == 1).And
                .ContainSingle(x => x.ChargeName.Value == "VatOnFee" && x.ChargeAmount.Value == 0.5m);
            orderItem2.Charges.Should()
                .HaveCount(7).And
                .ContainSingle(x => x.ChargeName.Value == "VatOnItem" && x.ChargeAmount.Value == 5).And
                .ContainSingle(x => x.ChargeName.Value == "VatOnDuty" && x.ChargeAmount.Value == 0.5m).And
                .ContainSingle(x => x.ChargeName.Value == "VatOnFee" && x.ChargeAmount.Value == 0.25m);
        }
    }
}
