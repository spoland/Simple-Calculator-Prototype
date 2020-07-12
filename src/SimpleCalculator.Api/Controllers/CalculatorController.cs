using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleCalculator.Api.Commands;
using SimpleCalculator.Api.Contracts;
using SimpleCalculator.Api.RequestHandlers;
using SimpleCalculator.Domain.Entities;
using SimpleCalculator.Domain.Models;
using SimpleCalculator.Domain.Options;
using SimpleCalculator.Domain.ValueObjects;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleCalculator.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CalculatorController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly List<CalculatorConfigurationOptions> _options;

        public CalculatorController(
            IMediator mediator,
            IOptionsSnapshot<List<CalculatorConfigurationOptions>> options)
        {
            _options = options.Value;
            _mediator = mediator;
        }

        [HttpPost("forward")]
        public async Task<ActionResult<IEnumerable<OrderChargeDto>>> ForwardCalculate(OrderDto requestDto)
        {
            var orderItems = requestDto.OrderItems.Select(oi =>
                new OrderItem(
                    quantity: new Quantity(oi.Quantity),
                    weight: Weight.InKilograms(oi.Weight),
                    vatRate: new Rate(oi.VatRate),
                    dutyRate: new Rate(oi.DutyRate),
                    inputPrice: new Price(oi.Price)))
                .ToList();

            var order = new Order(
                country: new Country(requestDto.CountryIso),
                currency: new Currency(requestDto.CurrencyIso),
                orderItems: orderItems);

            var calculatorConfiguration = CalculatorConfiguration.CreateFromOptions(_options.Single(x => x.Id == requestDto.CountryIso));

            var request = new ForwardCalculatorRequest(order, calculatorConfiguration);
            var response = await _mediator.Send(request);

            return new OkObjectResult(response);
        }

        [HttpPost("reverse")]
        public ActionResult<IEnumerable<OrderChargeDto>> ReverseCalculate(OrderDto request)
        {
            var orderItems = request.OrderItems.Select(oi =>
                    new OrderItem(
                        quantity: new Quantity(oi.Quantity),
                        weight: Weight.InKilograms(oi.Weight),
                        vatRate: new Rate(oi.VatRate),
                        dutyRate: new Rate(oi.DutyRate),
                        inputPrice: new Price(oi.Price)))
                .ToList();

            var order = new Order(
                country: new Country(request.CountryIso),
                currency: new Currency(request.CurrencyIso),
                orderItems: orderItems
            );

            ReverseCalculateCommand.Execute(order, _options.Single(x => x.Id == request.CountryIso));

            return new OkObjectResult(order.Charges.OrderBy(c => c.BaseChargeName.Value).Select(c => new OrderChargeDto(c.ChargeName, c.ChargeAmount)));
        }
    }
}