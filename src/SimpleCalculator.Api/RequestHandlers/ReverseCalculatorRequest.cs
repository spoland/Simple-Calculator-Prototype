using MediatR;
using SimpleCalculator.Api.Contracts;
using SimpleCalculator.Domain.Entities;
using SimpleCalculator.Domain.Models;
using System.Collections.Generic;

namespace SimpleCalculator.Api.RequestHandlers
{
    public class ReverseCalculatorRequest : IRequest<IEnumerable<OrderChargeResponse>>
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
