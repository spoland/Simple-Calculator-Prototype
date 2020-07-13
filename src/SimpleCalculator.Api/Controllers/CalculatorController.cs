using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleCalculator.Api.Contracts;
using SimpleCalculator.Api.Factories;
using SimpleCalculator.Api.Options;
using SimpleCalculator.Api.RequestHandlers;
using SimpleCalculator.Domain.Entities;
using SimpleCalculator.Domain.Models;
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
            var currency = new Currency(requestDto.CurrencyIso);
            var totalOrderCost = requestDto.OrderItems.Sum(x => x.Price);

            var orderItems = requestDto.OrderItems.Select(oi =>
                new OrderItem(
                    quantity: new Quantity(oi.Quantity),
                    weight: Weight.InKilograms(oi.Weight),
                    vatRate: new Rate(oi.VatRate),
                    dutyRate: new Rate(oi.DutyRate),
                    inputPrice: new Price(currency, oi.Price)))
                .ToList();

            var order = new Order(
                country: new Country(requestDto.CountryIso),
                currency: new Currency(requestDto.CurrencyIso),
                orderItems: orderItems);

            var config = _options.Single(x => x.Id == requestDto.CountryIso);
            var baseCharges = config.DeminimisBaseCharges.Select(chargeName => new ChargeName(chargeName));
            var chargeConfigurations = config.ChargeConfigurations.Select(config => ChargeConfigurationFactory.CreateFromOptions(config));

            var calculatorConfiguration = new CalculatorConfiguration(chargeConfigurations, baseCharges);

            var request = new ForwardCalculatorRequest(order, calculatorConfiguration);
            var response = await _mediator.Send(request);

            return new OkObjectResult(response);
        }

        [HttpPost("reverse")]
        public async Task<ActionResult<IEnumerable<OrderChargeDto>>> ReverseCalculate(OrderDto requestDto)
        {
            var currency = new Currency(requestDto.CurrencyIso);
            var totalOrderCost = requestDto.OrderItems.Sum(x => x.Price);

            var orderItems = requestDto.OrderItems.Select(oi =>
                    new OrderItem(
                        quantity: new Quantity(oi.Quantity),
                        weight: Weight.InKilograms(oi.Weight),
                        vatRate: new Rate(oi.VatRate),
                        dutyRate: new Rate(oi.DutyRate),
                        inputPrice: new Price(currency, oi.Price)))
                .ToList();

            var order = new Order(
                country: new Country(requestDto.CountryIso),
                currency: new Currency(requestDto.CurrencyIso),
                orderItems: orderItems
            );

            var config = _options.Single(x => x.Id == requestDto.CountryIso);
            var baseCharges = config.DeminimisBaseCharges.Select(chargeName => new ChargeName(chargeName));
            var chargeConfigurations = config.ChargeConfigurations.Select(config => ChargeConfigurationFactory.CreateFromOptions(config));

            var calculatorConfiguration = new CalculatorConfiguration(chargeConfigurations, baseCharges);

            var request = new ReverseCalculatorRequest(order, calculatorConfiguration);
            var response = await _mediator.Send(request);

            return new OkObjectResult(response);
        }
    }
}