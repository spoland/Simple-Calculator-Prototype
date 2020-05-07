using SimpleCalculator.Domain.Constants;
using SimpleCalculator.Domain.ValueObjects;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace SimpleCalculator.Domain.Models
{
    public class OrderItem
    {
        private readonly List<OrderCharge> _charges;

        public OrderItem(
            Quantity quantity,
            Weight weight,
            Rate vatRate,
            Rate dutyRate,
            Price price)
        {
            Weight = weight;
            Quantity = quantity;
            VatRate = vatRate;
            DutyRate = dutyRate;
            Price = price;

            _charges = new List<OrderCharge>();
        }

        /// <summary>
        /// The item quantity.
        /// </summary>
        public Quantity Quantity { get; }

        /// <summary>
        /// The item weight.
        /// </summary>
        public Weight Weight { get; }

        /// <summary>
        /// The items VAT rate.
        /// </summary>
        public Rate VatRate { get; }

        /// <summary>
        /// The items Duty rate.
        /// </summary>
        public Rate DutyRate { get; }

        /// <summary>
        /// The item price.
        /// </summary>
        public Price Price { get; }

        /// <summary>
        /// A collection of reverse rates that have been calculated during a reverse calculation.
        /// Make readonly and add method for adding/removing.
        /// </summary>
        public List<ReverseRate> ReverseRates { get; set; } = new List<ReverseRate>();

        /// <summary>
        /// The charges collection.
        /// </summary>
        public IEnumerable<OrderCharge> Charges => _charges;

        /// <summary>
        /// Get the requested order charge.
        /// </summary>
        /// <param name="chargeName">The charge name being requested.</param>
        /// <param name="includeSubCharges">A <see cref="bool"/> value indicating whether all sub charges (e.g: VatOnDuty, VatOnFee) should be included when requesting a charge (e.g Vat)</param>
        /// <returns></returns>
        public OrderCharge GetTotalCharge(ChargeName chargeName)
        {
            var chargeAmount = Charges.Where(c => c.ParentChargeName == chargeName)
                .Select(x => x.Charge)
                .Sum();

            return new OrderCharge(chargeName, chargeAmount, chargeName);
        }

        public void AddCharge(OrderCharge charge)
        {
            _charges.Add(charge);
        }

        public OrderCharge GetCharge(ChargeName chargeName)
        {
            return Charges.Single(c => c.Name == chargeName);
        }

        public OrderCharge GetTotalCalculatedCharge()
        {
            var totalCharge = _charges.Where(x => !x.Name.Value.Contains("Input")).Select(x => x.Charge).Sum();
            return new OrderCharge("Total", totalCharge, "Total");
        }

        public void RemoveCharge(ChargeName chargeName)
        {
            _charges.RemoveAll(x => x.Name == chargeName || x.ParentChargeName == chargeName);
        }

        public void ResetCalculationProperties()
        {
            _charges.RemoveAll(x => x.Name.Value != ChargeNames.InputItem);
            ReverseRates.Clear();
        }

        public void ResetKnownCharges()
        {
            _charges.RemoveAll(x => x.Name.Value != ChargeNames.InputItem && x.Name.Value != ChargeNames.Item);
        }
    }
}