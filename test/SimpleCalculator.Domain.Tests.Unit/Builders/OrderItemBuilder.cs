using SimpleCalculator.Domain.Models;
using SimpleCalculator.Domain.ValueObjects;

namespace SimpleCalculator.Domain.Tests.Unit.Builders
{
    //public class OrderItemBuilder
    //{
    //    private Quantity _quantity = new Quantity(0);
    //    private Price _price = new Price("EUR0");
    //    private Weight _weight = new Weight(1);
    //    private Percentage _vatRate = new Percentage(0);
    //    private Percentage _dutyRate = new Percentage(0);

    //    public OrderItemBuilder WithQuantity(int quantity) 
    //    {            
    //        _quantity = new Quantity(quantity);
    //        return this;
    //    }

    //    public OrderItemBuilder WithPrice(decimal price)
    //    {
    //        _price = new Price(new CurrencyIso("EUR"), price);
    //        return this;
    //    }

    //    public OrderItemBuilder WithWeight(decimal weight)
    //    {
    //        _weight = new Weight(weight);
    //        return this;
    //    }

    //    public OrderItemBuilder WithVatRate(decimal rate)
    //    {
    //        _vatRate = new Percentage(rate);
    //        return this;
    //    }

    //    public OrderItemBuilder WithDutyRate(decimal rate)
    //    {
    //        _dutyRate = new Percentage(rate);
    //        return this;
    //    }

    //    public OrderItem Build()
    //    {
    //        return new OrderItem(_quantity, _weight, _price, _vatRate, _dutyRate);
    //    }
    //}
}
