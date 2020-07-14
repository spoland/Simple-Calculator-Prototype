using SimpleCalculator.Domain.Abstractions;
using SimpleCalculator.Domain.Constants;
using SimpleCalculator.Domain.Models;
using SimpleCalculator.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleCalculator.Domain.Entities
{
    public class Order : IChargeable
    {
        private readonly List<OrderItem> _orderItems;

        public Order(Country country, Currency currency, List<OrderItem> orderItems, Price deliveryPrice)
        {
            Country = country;
            Currency = currency;
            _orderItems = orderItems;

            TotalOrderPrice = orderItems.Select(oi => oi.GetChargeAmount(ChargeNames.InputItem, currency)).Sum(currency);

            AddCharge(new OrderCharge(ChargeNames.InputDelivery, deliveryPrice, ChargeNames.InputDelivery, inputCharge: true));

            Id = new OrderId(Guid.NewGuid().ToString());
        }

        /// <summary>
        /// The order id
        /// </summary>
        public OrderId Id { get; }

        /// <summary>
        /// the delivery country ISO
        /// </summary>
        public Country Country { get; }

        /// <summary>
        /// The calculation currency ISO
        /// </summary>
        public Currency Currency { get; }

        /// <summary>
        /// The order items collection
        /// </summary>
        public IEnumerable<OrderItem> OrderItems => _orderItems;

        /// <summary>
        /// The charges collection
        /// </summary>
        public IEnumerable<OrderCharge> Charges => _orderItems.SelectMany(oi => oi.Charges);

        /// <summary>
        /// Get the requested order charge, if the charge name is a base charge it will return the total 
        /// charge amount including sub charges (Vat, Vat On Duty etc.)
        /// </summary>
        public Price GetChargeAmount(ChargeName chargeName, Currency currency)
        {
            return _orderItems.Select(oi => oi.GetChargeAmount(chargeName, currency)).Sum(currency);            
        }

        /// <summary>
        /// Removes the charge from all order items on the order.
        /// </summary>
        /// <param name="chargeName">Name of the charge.</param>
        public void RemoveCharge(ChargeName chargeName)
        {
            _orderItems.ForEach(oi => oi.RemoveCharge(chargeName));
        }

        public void ResetCalculationProperties()
        {
            foreach (var item in OrderItems) item.ResetCalculationProperties();
        }

        public decimal RelativeOrderItemValue(OrderItem orderItem) 
            => orderItem.GetChargeAmount(ChargeNames.InputItem, Currency).Value / TotalOrderPrice.Value;

        public void AddCharge(OrderCharge charge)
        {
            _orderItems.ForEach(oi => oi.AddCharge(
                new OrderCharge(
                    charge.ChargeName,
                    new Price(charge.ChargeAmount.Currency, charge.ChargeAmount.Value * RelativeOrderItemValue(oi)),
                    charge.BaseChargeName,
                    charge.InputCharge)));
        }

        private Price TotalOrderPrice;
    }
}