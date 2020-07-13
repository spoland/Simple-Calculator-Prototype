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
    public class ReverseCalculatorRequestHandler : IRequestHandler<ReverseCalculatorRequest, IEnumerable<OrderChargeResponse>>
    {
        public Task<IEnumerable<OrderChargeResponse>> Handle(ReverseCalculatorRequest request, CancellationToken cancellationToken)
        {
            // Loop through each range until correct one found
            foreach (var range in request.CalculatorConfiguration.CalculationRanges.Reverse())
            {
                // Create reverse calc using range
                var reverseCalculator = ReverseCalculatorFactory.Create(range);

                // Run reverse calc
                reverseCalculator?.Invoke(request.Order);

                // Create forward calc
                var forwardCalculator = ForwardCalculatorFactory.Create(range);

                // Run forward calc
                forwardCalculator?.Invoke(request.Order);

                // Get and compare charges
                var inputCharge = request.Order.GetCharge(ChargeNames.InputItem, request.Order.Currency);
                var totalCharge = request.Order.GetTotalCalculatedCharge();

                // Determine deminimis base
                var deminimisBase = request.Order.Charges.Where(chargeName => request.CalculatorConfiguration.DeminimisBaseCharges.Contains(chargeName.ChargeName))
                    .Select(x => x.ChargeAmount.Value).Sum();

                if (inputCharge.ChargeAmount == totalCharge.ChargeAmount && new Price(request.Order.Currency, deminimisBase) >= range.DeminimisThreshold)
                    break;

                // if charges don't match reset and run again
                request.Order.ResetCalculationProperties();
            }

            return Task.FromResult(request.Order.Charges
                .OrderBy(c => c.BaseChargeName.Value)
                .Select(c => new OrderChargeResponse(c.ChargeName, c.ChargeAmount)));
        }
    }
}
