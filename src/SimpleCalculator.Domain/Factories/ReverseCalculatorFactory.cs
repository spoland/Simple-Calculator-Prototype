using SimpleCalculator.Domain.Abstractions;
using SimpleCalculator.Domain.Calculators;
using SimpleCalculator.Domain.Calculators.Charge;
using SimpleCalculator.Domain.Constants;
using SimpleCalculator.Domain.Entities;
using SimpleCalculator.Domain.Exceptions;
using SimpleCalculator.Domain.Models;
using SimpleCalculator.Domain.Models.ChargeConfigurations;
using SimpleCalculator.Domain.ValueObjects;
using System.Collections.Generic;

namespace SimpleCalculator.Domain.Factories
{
    public static class ReverseCalculatorFactory
    {
        public delegate void Calculator(Order order);

        public static Calculator? Create(CalculationRange range, IEnumerable<ChargeName> deminimisBaseCharges)
        {
            Calculator? calculatorDelegate = null;

            foreach (var config in range.ChargeConfigurations)
            {
                IChargeCalculator calculator;

                switch (config)
                {
                    // FIXED RATE CONFIGURATIONS
                    case FixedRateChargeConfiguration fixedChargeConfig when config is FixedRateChargeConfiguration:
                        {
                            calculator = new FixedChargeCalculator(fixedChargeConfig.ChargeName, fixedChargeConfig.FixedChargeAmount);
                            break;
                        }

                    // WEIGHT BASED CONFIGURATIONS
                    case WeightBasedChargeConfiguration weightBasedChargeConfig when config is WeightBasedChargeConfiguration && weightBasedChargeConfig.Rate != null:
                        {
                            calculator = new WeightBasedChargeCalculator(weightBasedChargeConfig.ChargeName, (oi) => weightBasedChargeConfig.Rate);
                            break;
                        }
                    case WeightBasedChargeConfiguration weightBasedChargeConfig when config is WeightBasedChargeConfiguration && config.ChargeName.Value is ChargeNames.Duty:
                        {
                            calculator = new WeightBasedChargeCalculator(weightBasedChargeConfig.ChargeName, (oi) => oi.DutyRate);
                            break;
                        }
                    case WeightBasedChargeConfiguration weightBasedChargeConfig when config is WeightBasedChargeConfiguration && config.ChargeName.Value is ChargeNames.Vat:
                        {
                            calculator = new WeightBasedChargeCalculator(weightBasedChargeConfig.ChargeName, (oi) => oi.VatRate);
                            break;
                        }

                    // RATE BASED CONFIGURATIONS
                    case RateBasedChargeConfiguration rateBasedChargeConfig when config is RateBasedChargeConfiguration && rateBasedChargeConfig.Rate != null:
                        {
                            calculator = new ReverseRateCalculator(rateBasedChargeConfig.ChargeName, (oi) => rateBasedChargeConfig.Rate, rateBasedChargeConfig.BaseCharges);
                            break;
                        }
                    case RateBasedChargeConfiguration rateBasedChargeConfig when config is RateBasedChargeConfiguration && rateBasedChargeConfig.ChargeName.Value is ChargeNames.Duty:
                        {
                            calculator = new ReverseRateCalculator(rateBasedChargeConfig.ChargeName, (oi) => oi.DutyRate, rateBasedChargeConfig.BaseCharges);
                            break;
                        }
                    case RateBasedChargeConfiguration rateBasedChargeConfig when config is RateBasedChargeConfiguration && rateBasedChargeConfig.ChargeName.Value is ChargeNames.Vat:
                        {
                            calculator = new ReverseRateCalculator(rateBasedChargeConfig.ChargeName, (oi) => oi.VatRate, rateBasedChargeConfig.BaseCharges);
                            break;
                        }

                    default:
                        throw new InvalidChargeConfigurationException("Unknown charge configuration - can't create calculator delegate.");
                }

                // Add created calculator to delegate
                calculatorDelegate += calculator.Calculate;
            }

            foreach(var deminimisBaseCharge in deminimisBaseCharges)
            {
                // Add the item price calculator last
                calculatorDelegate += new ReversePriceCalculator(deminimisBaseCharge, $"Input{deminimisBaseCharge}").Calculate;
            }

            return calculatorDelegate;
        }
    }
}
