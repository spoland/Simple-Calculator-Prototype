using MediatR;
using SimpleCalculator.Api.Contracts;
using SimpleCalculator.Domain.Constants;
using SimpleCalculator.Domain.Factories;
using SimpleCalculator.Domain.ValueObjects;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleCalculator.Api.RequestHandlers
{
    public class ForwardCalculatorRequestHandler : IRequestHandler<ForwardCalculatorRequest, OrderResponse>
    {
        public Task<OrderResponse> Handle(ForwardCalculatorRequest request, CancellationToken cancellationToken)
        {
            // Add initial charges
            foreach (var item in request.Order.OrderItems)
            {
                var inputItemPrice = item.GetChargeAmount(ChargeNames.InputItem, request.Order.Currency);
                var inputDeliveryPrice = item.GetChargeAmount(ChargeNames.InputDelivery, request.Order.Currency);

                item.AddCharge(new OrderCharge(ChargeNames.Item, inputItemPrice, ChargeNames.Item));
                item.AddCharge(new OrderCharge(ChargeNames.Delivery, inputDeliveryPrice, ChargeNames.Delivery));
            };

            // Determine deminimis base
            var deminimisBase = request.Order.Charges.Where(chargeName => request.CalculatorConfiguration.DeminimisBaseCharges.Contains(chargeName.ChargeName))
                .Select(x => x.ChargeAmount.Value).Sum();

            // Get the correct range for the base price
            var range = request.CalculatorConfiguration.GetRangeForBasePrice(new Price(request.Order.Currency, deminimisBase));

            // Create a forward calculator for the selected range
            var calculator = ForwardCalculatorFactory.Create(range);

            // Run calculator
            calculator?.Invoke(request.Order);

            return Task.FromResult(new OrderResponse(request.Order));
        }
    }
}
