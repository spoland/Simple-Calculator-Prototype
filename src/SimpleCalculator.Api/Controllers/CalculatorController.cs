using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleCalculator.Api.Commands;
using SimpleCalculator.Api.Contracts;
using SimpleCalculator.Domain.Constants;
using SimpleCalculator.Domain.Entities;
using SimpleCalculator.Domain.Models;
using SimpleCalculator.Domain.Options;
using SimpleCalculator.Domain.ValueObjects;
using System.Collections.Generic;
using System.Linq;

namespace SimpleCalculator.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CalculatorController : ControllerBase
    {
        private readonly List<CalculatorConfigurationOptions> _options;
        
        public CalculatorController(
            IOptionsSnapshot<List<CalculatorConfigurationOptions>> options)
        {
            _options = options.Value;
        }

        [HttpPost("forward")]
        public ActionResult<IEnumerable<OrderChargeDto>> ForwardCalculate(OrderDto request)
        {
            var orderItems = request.OrderItems.Select(oi =>
                new OrderItem(
                    quantity: new Quantity(oi.Quantity),
                    weight: Weight.InKilograms(oi.Weight),
                    vatRate: new Rate(oi.VatRate),
                    dutyRate: new Rate(oi.DutyRate),
                    inputPrice: new Price(oi.Price)))
                .ToList();
            
            orderItems.ForEach(oi =>
            {
                oi.AddCharge(new OrderCharge(ChargeNames.InputItem, new Price(oi.Price.Currency, oi.Price.Amount), ChargeNames.InputItem));
                oi.AddCharge(new OrderCharge(ChargeNames.Item, new Price(oi.Price.Currency, oi.Price.Amount), ChargeNames.Item));
            });

            var order = new Order(
                country: new Country(request.CountryIso),
                currency: new Currency(request.CurrencyIso),
                orderItems: orderItems);

            ForwardCalculateCommand.Execute(order, _options.Single(x => x.Id == request.CountryIso));

            return new OkObjectResult(order.Charges.OrderBy(c => c.BaseChargeName.Value).Select(c => new OrderChargeDto(c)));
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

            orderItems.ForEach(oi =>
            {
                oi.AddCharge(new OrderCharge(ChargeNames.InputItem, oi.Price, ChargeNames.InputItem));
            });

            var order = new Order(
                country: new Country(request.CountryIso),
                currency: new Currency(request.CurrencyIso),
                orderItems: orderItems
            );

            ReverseCalculateCommand.Execute(order, _options.Single(x => x.Id == request.CountryIso));

            return new OkObjectResult(order.Charges.OrderBy(c => c.BaseChargeName.Value).Select(c => new OrderChargeDto(c)));
        }
    }
}