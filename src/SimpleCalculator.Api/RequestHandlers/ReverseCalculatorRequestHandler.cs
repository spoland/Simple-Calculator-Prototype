using MediatR;
using SimpleCalculator.Api.Contracts;
using SimpleCalculator.Domain.Constants;
using SimpleCalculator.Domain.Entities;
using SimpleCalculator.Domain.Factories;
using SimpleCalculator.Domain.Models;
using SimpleCalculator.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleCalculator.Api.RequestHandlers
{
    public class ReverseCalculatorRequestHandler : IRequestHandler<ReverseCalculatorRequest, OrderResponse>
    {
        public Task<OrderResponse> Handle(ReverseCalculatorRequest request, CancellationToken cancellationToken)
        {
            var ranges = request.CalculatorConfiguration.CalculationRanges.Reverse();
            var deliveryInDeminimisBase = request.CalculatorConfiguration.DeminimisBaseCharges.Contains(new ChargeName(ChargeNames.Delivery));

            DetectGreyZones(request.CalculatorConfiguration, request.Order);

            var inputCharge = request.Order.Charges.Where(x => x.ChargeName == ChargeNames.InputItem).Select(x => x.ChargeAmount.Value).Sum();
            if (deliveryInDeminimisBase) inputCharge += request.Order.Charges.Where(x => x.ChargeName == ChargeNames.InputDelivery).Select(x => x.ChargeAmount.Value).Sum();

            if (request.CalculatorConfiguration.InputPriceInGreyZone(new Price(request.Order.Currency, inputCharge), out var greyZoneRange))
            {
                Calculate(greyZoneRange, request.Order, deliveryInDeminimisBase, request.CalculatorConfiguration.DeminimisBaseCharges);
            } 
            else
            {
                foreach (var range in ranges)
                {
                    Calculate(range, request.Order, deliveryInDeminimisBase, request.CalculatorConfiguration.DeminimisBaseCharges);

                    // Get and compare charges
                    var totalCalculatedChargeAmount = request.Order.Charges
                        .Where(x => !x.InputCharge)
                        .Select(x => x.ChargeAmount)
                        .Sum(request.Order.Currency);

                    var totalInputChargeAmount = request.Order.Charges
                        .Where(x => x.InputCharge)
                        .Select(x => x.ChargeAmount)
                        .Sum(request.Order.Currency);

                    // Determine deminimis base
                    var deminimisBase = request.Order.Charges
                        .Where(chargeName => request.CalculatorConfiguration.DeminimisBaseCharges.Contains(chargeName.ChargeName))
                        .Select(x => x.ChargeAmount.Value).Sum();

                    // If the value of the base charges in this range are greater than the current ranges threshold - we're finished.
                    if (deminimisBase >= range.DeminimisThreshold.Value)
                        break;

                    // Reset charges and run again
                    request.Order.ResetCalculationProperties();
                }
            }

            foreach (var item in request.Order.OrderItems)
            {
                var totalItemPrice = item.Charges
                    .Where(x => !x.InputCharge)
                    .Where(x => x.ChargeName.Value.EndsWith("Item"))
                    .Select(x => x.ChargeAmount.Value)
                    .Sum();

                var totalDeliveryPrice = item.Charges
                    .Where(x => !x.InputCharge)
                    .Where(x => x.ChargeName.Value.EndsWith("Delivery"))
                    .Select(x => x.ChargeAmount.Value)
                    .Sum();

                item.AddCharge(new OrderCharge("TotalItem", new Price(request.Order.Currency, totalItemPrice), "TotalItem"));
                item.AddCharge(new OrderCharge("TotalDelivery", new Price(request.Order.Currency, totalDeliveryPrice), "TotalDelivery"));
            }

            return Task.FromResult(new OrderResponse(request.Order));
        }

        private void Calculate(
            CalculationRange range,
            Order order,
            bool deliveryInDeminimisBase,
            IEnumerable<ChargeName> deminimisBaseCharges)
        {
            // If no delivery reversal is required, add to charges
            if (!deliveryInDeminimisBase)
            {
                var inputDeliveryPrice = order.GetChargeAmount(ChargeNames.InputDelivery, order.Currency);
                order.AddCharge(new OrderCharge(ChargeNames.Delivery, inputDeliveryPrice, ChargeNames.Delivery));
            }

            // Create reverse calc using range
            var reverseCalculator = ReverseCalculatorFactory.Create(range, deminimisBaseCharges);

            // Run reverse calc
            reverseCalculator?.Invoke(order);

            // Create forward calc
            var forwardCalculator = ForwardCalculatorFactory.Create(range, includeFixedCalculators: false);

            // Run forward calc
            forwardCalculator?.Invoke(order);
        }

        // TODO: Create abstraction
        private void DetectGreyZones(CalculatorConfiguration configuration, Order order)
        {
            var boundaries = new List<Tuple<Price, CalculationRange>>();

            foreach (var range in configuration.CalculationRanges.Where(x => x.DeminimisThreshold.Value != 0))
            {
                boundaries.Add(new Tuple<Price, CalculationRange>(new Price(order.Currency, range.DeminimisThreshold.Value - .01m), range));
                boundaries.Add(new Tuple<Price, CalculationRange>(new Price(order.Currency, range.DeminimisThreshold.Value), range));
            }

            var greyZonePrices = new List<Tuple<Price, CalculationRange>>();

            foreach (var boundary in boundaries)
            {
                // Create forward calc
                var range = configuration.GetRangeForBasePrice(boundary.Item1);
                var forwardCalculator = ForwardCalculatorFactory.Create(range);

                var orderItems = order.OrderItems.Select(oi => new OrderItem(oi.Quantity, oi.Weight, oi.VatRate, oi.DutyRate, new Price(order.Currency, order.RelativeOrderItemValue(oi) * boundary.Item1.Value))).ToList();
                orderItems.ForEach(oi => { var c = oi.GetChargeAmount(ChargeNames.InputItem, order.Currency); oi.AddCharge(new OrderCharge(ChargeNames.Item, c, ChargeNames.Item)); });
                var detectionOrder = new Order(order.Country, order.Currency, orderItems, new Price(order.Currency, 0));

                forwardCalculator?.Invoke(detectionOrder);

                var greyZonePrice = detectionOrder.Charges
                    .Where(x => !x.InputCharge)
                    .Where(x => x.ChargeName.Value.EndsWith(ChargeNames.Item))
                    .Select(x => x.ChargeAmount.Value)
                    .Sum();

                greyZonePrices.Add(new Tuple<Price, CalculationRange>(new Price(order.Currency, greyZonePrice), boundary.Item2));
            }

            if (greyZonePrices.Count % 2 != 0) throw new ArgumentException("Invalid grey zones, should be in pairs!");

            for (int x = 0; x < greyZonePrices.Count; x++)
            {
                configuration.AddGreyZone(new GreyZone(greyZonePrices[x].Item1, greyZonePrices[x + 1].Item1, greyZonePrices[x].Item2));
                x++;
            }
        }
    }
}
