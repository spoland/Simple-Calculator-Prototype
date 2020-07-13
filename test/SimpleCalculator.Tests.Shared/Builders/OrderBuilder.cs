using SimpleCalculator.Domain.Entities;
using SimpleCalculator.Domain.Models;
using SimpleCalculator.Domain.ValueObjects;
using SimpleCalculator.Tests.Shared.Mocks;
using System.Collections.Generic;

namespace SimpleCalculator.Tests.Shared.Builders
{
    public class OrderBuilder
    {
        private Country? _country;
        private Currency? _currency;
        private Price? _deliveryPrice;

        private List<OrderItem> _orderItems = new List<OrderItem>();

        public OrderBuilder WithCountry(string countryIso)
        {
            _country = new Country(countryIso);
            return this;
        }

        public OrderBuilder WithCurrency(string currencyIso)
        {
            _currency = new Currency(currencyIso);
            return this;
        }

        public OrderBuilder WithOrderItems(List<OrderItem> orderItems)
        {
            _orderItems = orderItems;
            return this;
        }

        public OrderBuilder WithDeliveryPrice(Price deliveryPrice)
        {
            _deliveryPrice = deliveryPrice;
            return this;
        }

        public Order Build()
        {
            return new Order(
                country: _country ?? CountryFakes.IE,
                currency: _currency ?? CurrencyFakes.EUR,
                orderItems: _orderItems,
                deliveryPrice: _deliveryPrice ?? "EUR100");
        }

    }
}
