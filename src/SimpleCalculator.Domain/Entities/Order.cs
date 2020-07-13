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
        public Order(Country country, Currency currency, List<OrderItem> orderItems)
        {
            CountryIso = country;
            CurrencyIso = currency;
            OrderItems = orderItems;

            TotalOrderPrice = orderItems.Select(oi => oi.GetCharge(ChargeNames.InputItem).ChargeAmount).Sum();

            Id = new OrderId(Guid.NewGuid().ToString());
        }

        /// <summary>
        /// The order id
        /// </summary>
        public OrderId Id { get; }

        /// <summary>
        /// the delivery country ISO
        /// </summary>
        public Country CountryIso { get; }

        /// <summary>
        /// The calculation currency ISO
        /// </summary>
        public Currency CurrencyIso { get; }

        /// <summary>
        /// The order items collection
        /// </summary>
        public IEnumerable<OrderItem> OrderItems { get; }

        /// <summary>
        /// The charges collection
        /// </summary>
        public IEnumerable<OrderCharge> Charges => OrderItems.SelectMany(oi => oi.Charges);

        /// <summary>
        /// Get the requested order charge, if the charge name is a base charge it will return the total 
        /// charge amount including sub charges (Vat, Vat On Duty etc.)
        /// </summary>
        public OrderCharge GetCharge(ChargeName chargeName)
        {
            var chargeAmount = OrderItems.Select(oi => oi.GetCharge(chargeName).ChargeAmount).Sum();
            return new OrderCharge(chargeName, chargeAmount, chargeName);
        }

        public OrderCharge GetTotalCalculatedCharge()
        {
            var totalOrderCharges = OrderItems.Select(oi => oi.GetTotalCalculatedCharge()).Select(x => x.ChargeAmount).Sum();
            return new OrderCharge("Total", totalOrderCharges, "Total");
        }

        public void ResetCalculationProperties()
        {
            foreach (var item in OrderItems) item.ResetCalculationProperties();
        }

        public decimal RelativeOrderItemValue(OrderItem orderItem) 
            => orderItem.GetCharge(ChargeNames.InputItem).ChargeAmount.Value / TotalOrderPrice.Value;

        private Price TotalOrderPrice;
    }
}