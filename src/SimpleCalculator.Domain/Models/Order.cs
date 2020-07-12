using SimpleCalculator.Domain.Constants;
using SimpleCalculator.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleCalculator.Domain.Models
{
    public class Order
    {
        private readonly Price _totalOrderPrice;

        public Order(Country countryIso, Currency currencyIso, IEnumerable<OrderItem> orderItems)
        {
            CountryIso = countryIso;
            CurrencyIso = currencyIso;
            OrderItems = orderItems;

            _totalOrderPrice = orderItems
                .SelectMany(x => x.Charges)
                .Where(x => x.Name.Value == ChargeNames.InputItem)
                .Select(c => c.Charge)
                .Sum();

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
        /// Get the requested order charge - does not include subcharges (e.g VatOnDuty)
        /// </summary>
        /// <param name="chargeName">The charge name being requested.</param>
        /// <param name="includeSubCharges">A <see cref="bool"/> value indicating whether all sub charges (e.g: VatOnDuty, VatOnFee) should be included when requesting a charge (e.g Vat)</param>
        /// <returns></returns>
        public OrderCharge GetCharge(ChargeName chargeName)
        {
            var chargeAmount = OrderItems.Select(oi => oi.GetCharge(chargeName).Charge).Sum();
            return new OrderCharge(chargeName, chargeAmount, chargeName);
        }

        /// <summary>
        /// Get the requested order charge, including sub charges.
        /// </summary>
        /// <param name="chargeName">The charge name being requested.</param>
        /// <param name="includeSubCharges">A <see cref="bool"/> value indicating whether all sub charges (e.g: VatOnDuty, VatOnFee) should be included when requesting a charge (e.g Vat)</param>
        /// <returns></returns>
        public OrderCharge GetTotalCharge(ChargeName chargeName)
        {
            var chargeAmount = OrderItems.Select(oi => oi.GetTotalCharge(chargeName).Charge).Sum();
            return new OrderCharge(chargeName, chargeAmount, chargeName);
        }

        public OrderCharge GetTotalCalculatedCharge()
        {
            var totalOrderCharges = OrderItems.Select(oi => oi.GetTotalCalculatedCharge()).Select(x => x.Charge).Sum();
            return new OrderCharge("Total", totalOrderCharges, "Total");
        }

        public Rate RelativeItemValue(OrderItem orderItem)
            => new Rate(orderItem.GetCharge(ChargeNames.InputItem).Charge.Amount / _totalOrderPrice.Amount * 100);

        public void ResetCalculationProperties()
        {
            foreach (var item in OrderItems) item.ResetCalculationProperties();
        }
    }
}