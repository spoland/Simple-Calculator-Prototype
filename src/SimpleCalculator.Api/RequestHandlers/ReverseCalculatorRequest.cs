using MediatR;
using SimpleCalculator.Api.Contracts;
using SimpleCalculator.Domain.Entities;
using SimpleCalculator.Domain.Models;

namespace SimpleCalculator.Api.RequestHandlers
{
    public class ReverseCalculatorRequest : IRequest<OrderResponse>
    {
        public ReverseCalculatorRequest(Order order, CalculatorConfiguration calculatorConfiguration)
        {
            Order = order;
            CalculatorConfiguration = calculatorConfiguration;
        }

        public Order Order { get; }

        public CalculatorConfiguration CalculatorConfiguration { get; }
    }
}
