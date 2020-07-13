using MediatR;
using SimpleCalculator.Api.Contracts;
using SimpleCalculator.Domain.Constants;
using SimpleCalculator.Domain.Factories;
using SimpleCalculator.Domain.ValueObjects;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleCalculator.Api.RequestHandlers
{
    public class ForwardCalculatorRequestHandler : IRequestHandler<ForwardCalculatorRequest, IEnumerable<OrderChargeResponse>>
    {
        public Task<IEnumerable<OrderChargeResponse>> Handle(ForwardCalculatorRequest request, CancellationToken cancellationToken)
        {
            // Determine deminimis base
            var deminimisBase = request.Order.Charges.Where(chargeName => request.CalculatorConfiguration.DeminimisBaseCharges.Contains(chargeName.ChargeName))
                .Select(x => x.ChargeAmount.Value).Sum();

            // Get the correct range for the base price
            var range = request.CalculatorConfiguration.GetRangeForBasePrice(new Price(request.Order.Currency, deminimisBase));

            // Create a forward calculator for the selected range
            var calculator = ForwardCalculatorFactory.Create(range);

            foreach (var item in request.Order.OrderItems)
            {
                var inputItemPrice = item.GetChargeAmount(ChargeNames.InputItem, request.Order.Currency);
                var inputDeliveryPrice = item.GetChargeAmount(ChargeNames.InputDelivery, request.Order.Currency);

                item.AddCharge(new OrderCharge(ChargeNames.Item, inputItemPrice, ChargeNames.Item));
                item.AddCharge(new OrderCharge(ChargeNames.Delivery, inputDeliveryPrice, ChargeNames.Delivery));
            };

            // Run calculator
            calculator?.Invoke(request.Order);

            // Add a total charge for visibility
            foreach (var item in request.Order.OrderItems)
            {
                var totalItemCharge = item.GetTotalCalculatedCharge(request.Order.Currency);
                item.AddCharge(totalItemCharge);
            }

            return Task.FromResult(request.Order.Charges
                .OrderBy(c => c.BaseChargeName.Value)
                .Select(c => new OrderChargeResponse(c.ChargeName, c.ChargeAmount)));
        }
    }
}
