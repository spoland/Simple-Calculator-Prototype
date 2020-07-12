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
    public class ForwardCalculatorRequestHandler : IRequestHandler<ForwardCalculatorRequest, IEnumerable<OrderChargeDto>>
    {
        public Task<IEnumerable<OrderChargeDto>> Handle(ForwardCalculatorRequest request, CancellationToken cancellationToken)
        {
            // Determine deminimis base
            var deminimisBase = request.Order.Charges.Where(chargeName => request.CalculatorConfiguration.DeminimisBaseCharges.Contains(chargeName.ChargeName))
                .Select(x => x.ChargeAmount.Value).Sum();

            // Get the correct range for the base price
            var range = request.CalculatorConfiguration.GetRangeForBasePrice(new Price(request.Order.CurrencyIso, deminimisBase));

            // Create a forward calculator for the selected range
            var calculator = ForwardCalculatorFactory.Create(range, request.CalculatorConfiguration.Excess);

            foreach (var item in request.Order.OrderItems)
            {
                var inputPrice = item.GetCharge(ChargeNames.InputItem);
                item.AddCharge(new OrderCharge(ChargeNames.Item, inputPrice.ChargeAmount, ChargeNames.Item));
            };

            // Run calculator
            calculator?.Invoke(request.Order);

            // Add a total charge for visibility
            foreach (var item in request.Order.OrderItems)
            {
                var totalItemCharge = item.GetTotalCalculatedCharge();
                item.AddCharge(totalItemCharge);
            }

            return Task.FromResult(request.Order.Charges
                .OrderBy(c => c.BaseChargeName.Value)
                .Select(c => new OrderChargeDto(c.ChargeName, c.ChargeAmount)));
        }
    }
}
