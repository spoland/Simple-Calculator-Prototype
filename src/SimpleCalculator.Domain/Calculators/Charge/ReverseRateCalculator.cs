using SimpleCalculator.Domain.Abstractions;
using SimpleCalculator.Domain.Entities;
using SimpleCalculator.Domain.Models;
using SimpleCalculator.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleCalculator.Domain.Calculators
{
    public class ReverseRateCalculator : IChargeCalculator
    {
        private readonly ChargeName _chargeName;
        private readonly Func<OrderItem, Rate> _getRate;
        private readonly IEnumerable<ChargeName> _baseCharges;

        /// <summary>
        /// Create a new <see cref="ReverseRateCalculator"/>.
        /// </summary>
        /// <param name="chargeName">The charge name that the reverse rate is being calculated for.</param>
        /// <param name="getRate">A func used to determine the rate for each item.</param>
        /// <param name="baseCharges">A collection of <see cref="ChargeName"/>, used to determine which charges this charge should be applied on top of.</param>
        public ReverseRateCalculator(
            ChargeName chargeName,
            Func<OrderItem, Rate> getRate,
            IEnumerable<ChargeName> baseCharges)
        {
            _getRate = getRate;
            _baseCharges = baseCharges;
            _chargeName = chargeName;
        }

        public void Calculate(Order order)
        {
            foreach (var item in order.OrderItems)
            {
                // Loop through each of the charges that this charge type is to be calculated on top of.
                foreach (var baseChargeName in _baseCharges)
                {
                    var baseChargeAmount = item.GetTotalCharge(baseChargeName);

                    // Check to see if a charge has been calculated for this charge type.
                    // If so, do a forward calculation - the calculated charges can be removed from the 
                    // inclusive price before applying the reverse rate as they are known.
                    if (baseChargeAmount.ChargeAmount.Amount != 0)
                    {
                        var chargeAmount = baseChargeAmount.ChargeAmount * _getRate(item).AsDecimal;

                        var chargeName = ChargeName.FromBaseChargeName(_chargeName, baseChargeName);

                        item.AddCharge(new OrderCharge(chargeName, chargeAmount, _chargeName));
                    }

                    // If the charge that we need to calculate on top of has not already been calculated,
                    // and is therefore unknown - we need to calculate the reverse rate.
                    else
                    {
                        // If the base charge is item or delivery (not a calculated rate), just add the rate
                        // for this item to the calculation rates collection. There might be a better way of
                        // identifying these 'non calculated' rates types of charges that would be more flexible.
                        if (baseChargeName.Value == "Item" || baseChargeName.Value == "Delivery")
                        {
                            item.AddReverseRate(new ReverseRate(
                                name: ChargeName.FromBaseChargeName(_chargeName, baseChargeName),
                                parentChargeName: _chargeName,
                                rate: _getRate(item)));
                        }                           

                        // If the base charge is a calculated rate, we need to calculate a new rate and add it
                        // to the calculated rates collection. All of these rates combined will be used to reverse
                        // back to the item price.
                        else
                        {
                            // Get the current list of rates
                            var reverseRates = item.ReverseRates.ToList();

                            // Get all of the reverse rates related to the current base charge. For example, if we're
                            // calculating a fee, which was calculated on top of duty and vat, we'll need the base
                            // fee rate, the fee on duty rate and the fee on vat rate. This is done by querying the
                            // rates collection by 'parent' charge. In this example, all rates would have a parent charge
                            // of fee.
                            foreach(var reverseRate in reverseRates.Where(x => x.BaseChargeName == baseChargeName))
                            {                                
                                var rate = _getRate(item).AsDecimal * reverseRate.Rate.AsDecimal * 100;

                                var calculatedRate = new ReverseRate(
                                     name: ChargeName.FromBaseChargeName(_chargeName, reverseRate.ChargeName),
                                     parentChargeName: _chargeName,
                                     rate: new Rate(rate));

                                item.AddReverseRate(calculatedRate);
                            }                            
                        }
                    }
                }
            }
        }
    }
}