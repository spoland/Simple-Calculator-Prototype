using SimpleCalculator.Domain.Models;
using SimpleCalculator.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleCalculator.Domain.Tests.Unit.Builders
{
    //public class OrderBuilder
    //{
    //    private OrderId _orderId = new OrderId(Guid.NewGuid().ToString());
    //    private CountryIso _countryIso = new CountryIso("IE");
    //    private IEnumerable<OrderItem> _orderItems = Enumerable.Empty<OrderItem>();

    //    public OrderBuilder WithOrderId(string id)
    //    {
    //        _orderId = new OrderId(id);
    //        return this;
    //    }

    //    public OrderBuilder WithCountryIso(string iso)
    //    {
    //        _countryIso = new CountryIso(iso);
    //        return this;
    //    }

    //    public OrderBuilder WithOrderItems(IEnumerable<OrderItem> orderItems)
    //    {
    //        _orderItems = orderItems;
    //        return this;
    //    }

    //    public Order Build()
    //    {
    //        return new Order(_orderId, _countryIso, _orderItems);
    //    }
    //}
}
