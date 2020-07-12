using Moq;
using SimpleCalculator.Domain.Models;
using SimpleCalculator.Domain.ValueObjects;
using SimpleCalculator.Tests.Shared.Mocks;

namespace SimpleCalculator.Tests.Shared.Builders
{
    public class OrderItemBuilder
    {
        private Price? _inputPrice;
        private Quantity? _quantity;
        private Rate? _vatRate;
        private Rate? _dutyRate;
        private Weight? _weight;

        private readonly Currency DefaultCurrency = CurrencyFakes.EUR;

        public OrderItemBuilder WithQuantity(int quantity)
        {
            _quantity = new Quantity(quantity);
            return this;
        }

        public OrderItemBuilder WithPrice(string currencyIso, decimal price)
        {
            _inputPrice = new Price(new Currency(currencyIso), price);
            return this;
        }

        public OrderItemBuilder WithWeight(decimal weightInKg)
        {
            _weight = Weight.InKilograms(weightInKg);
            return this;
        }

        public OrderItemBuilder WithVatRate(decimal rate)
        {
            _vatRate = new Rate(rate);
            return this;
        }

        public OrderItemBuilder WithDutyRate(decimal rate)
        {
            _dutyRate = new Rate(rate);
            return this;
        }

        public OrderItem Build()
        {
            return new OrderItem(
                quantity: _quantity ?? new Quantity(1),
                weight: _weight ?? Weight.InKilograms(1),
                vatRate: _vatRate ?? new Rate(1),
                dutyRate: _dutyRate ?? new Rate(1),
                inputPrice: _inputPrice ?? new Price(DefaultCurrency, 100));
        }
    }
}
