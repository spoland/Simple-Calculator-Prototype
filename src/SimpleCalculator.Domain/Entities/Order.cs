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

        public Order(Country country, Currency currency, List<OrderItem> orderItems)
        {
            Country = country;
            Currency = currency;
            _orderItems = orderItems;

            TotalOrderPrice = orderItems.Select(oi => oi.GetCharge(ChargeNames.InputItem, currency).ChargeAmount).Sum(currency);

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
        public OrderCharge GetCharge(ChargeName chargeName, Currency currency)
        {
            var chargeAmount = _orderItems.Select(oi => oi.GetCharge(chargeName, currency).ChargeAmount).Sum(currency);
            return new OrderCharge(chargeName, chargeAmount, chargeName);
        }

        /// <summary>
        /// Removes the charge from all order items on the order.
        /// </summary>
        /// <param name="chargeName">Name of the charge.</param>
        public void RemoveCharge(ChargeName chargeName)
        {
            _orderItems.ForEach(oi => oi.RemoveCharge(chargeName));
        }

        public OrderCharge GetTotalCalculatedCharge()
        {
            var totalOrderCharges = OrderItems.Select(oi => oi.GetTotalCalculatedCharge(Currency)).Select(x => x.ChargeAmount).Sum(Currency);
            return new OrderCharge("Total", totalOrderCharges, "Total");
        }

        public void ResetCalculationProperties()
        {
            foreach (var item in OrderItems) item.ResetCalculationProperties();
        }

        public decimal RelativeOrderItemValue(OrderItem orderItem) 
            => orderItem.GetCharge(ChargeNames.InputItem, Currency).ChargeAmount.Value / TotalOrderPrice.Value;

        private Price TotalOrderPrice;
    }
}