using SimpleCalculator.Domain.Abstractions;
using SimpleCalculator.Domain.Constants;
using SimpleCalculator.Domain.ValueObjects;
using System.Collections.Generic;
using System.Linq;

namespace SimpleCalculator.Domain.Models
{
    public class OrderItem : IChargeable
    {
        private readonly List<OrderCharge> _charges;
        private readonly List<ReverseRate> _reverseRates;

        public OrderItem(
            Quantity quantity,
            Weight weight,
            Rate vatRate,
            Rate dutyRate,
            Price inputPrice)
        {
            Weight = weight;
            Quantity = quantity;
            VatRate = vatRate;
            DutyRate = dutyRate;

            _charges = new List<OrderCharge>();
            _reverseRates = new List<ReverseRate>();

            _charges.Add(new OrderCharge(ChargeNames.InputItem, inputPrice));
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
        /// The charges collection.
        /// </summary>
        public IEnumerable<OrderCharge> Charges => _charges;

        /// <summary>
        /// A collection of reverse rates that have been calculated during a reverse calculation.
        /// Make readonly and add method for adding/removing.
        /// </summary>
        public IEnumerable<ReverseRate> ReverseRates => _reverseRates;

        /// <summary>
        /// Get the requested order charge.
        /// </summary>
        /// <param name="chargeName">The charge name being requested.</param>
        /// <param name="includeSubCharges">A <see cref="bool"/> value indicating whether all sub charges (e.g: VatOnDuty, VatOnFee) should be included when requesting a charge (e.g Vat)</param>
        /// <returns></returns>
        public OrderCharge GetCharge(ChargeName chargeName)
        {
            var chargeAmount = Charges
                .Where(c => c.BaseChargeName == chargeName || c.BaseChargeName == ChargeName.Empty && c.ChargeName == chargeName)
                .Select(x => x.ChargeAmount)
                .Sum();

            return new OrderCharge(chargeName, chargeAmount);
        }

        public void AddCharge(OrderCharge charge)
        {
            _charges.Add(charge);
        }

        public OrderCharge GetTotalCalculatedCharge()
        {
            var totalCharge = _charges.Where(x => !x.ChargeName.Value.Contains("Input")).Select(x => x.ChargeAmount).Sum();
            return new OrderCharge("Total", totalCharge, "Total");
        }

        public void RemoveCharge(ChargeName chargeName)
        {
            _charges.RemoveAll(x => x.ChargeName == chargeName || x.BaseChargeName == chargeName);
        }

        public void ResetCalculationProperties()
        {
            _charges.RemoveAll(x => x.ChargeName.Value != ChargeNames.InputItem);
            _reverseRates.Clear();
        }

        public void ResetKnownCharges()
        {
            _charges.RemoveAll(x => x.ChargeName.Value != ChargeNames.InputItem && x.ChargeName.Value != ChargeNames.Item);
        }

        public void AddReverseRate(ReverseRate reverseRate) => _reverseRates.Add(reverseRate);
    }
}