using SimpleCalculator.Domain.Calculators;
using SimpleCalculator.Domain.Calculators.Charge;
using SimpleCalculator.Domain.Constants;
using SimpleCalculator.Domain.Exceptions;
using SimpleCalculator.Domain.Models;
using SimpleCalculator.Domain.Models.ChargeConfigurations;

namespace SimpleCalculator.Domain.Factories
{
    public static class ReverseCalculatorFactory
    {
        public delegate void Calculator(Order order);

        public static Calculator? Create(CalculationRange range)
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
                            calculator = new FixedChargeCalculator(fixedChargeConfig.Name, fixedChargeConfig.FixedChargeAmount);
                            break;
                        }

                    // WEIGHT BASED CONFIGURATIONS
                    case WeightBasedChargeConfiguration weightBasedChargeConfig when config is WeightBasedChargeConfiguration && weightBasedChargeConfig.Rate != null:
                        {
                            calculator = new WeightBasedChargeCalculator(weightBasedChargeConfig.Name, (oi) => weightBasedChargeConfig.Rate);
                            break;
                        }
                    case WeightBasedChargeConfiguration weightBasedChargeConfig when config is WeightBasedChargeConfiguration && config.Name.Value is ChargeNames.Duty:
                        {
                            calculator = new WeightBasedChargeCalculator(weightBasedChargeConfig.Name, (oi) => oi.DutyRate);
                            break;
                        }
                    case WeightBasedChargeConfiguration weightBasedChargeConfig when config is WeightBasedChargeConfiguration && config.Name.Value is ChargeNames.Vat:
                        {
                            calculator = new WeightBasedChargeCalculator(weightBasedChargeConfig.Name, (oi) => oi.VatRate);
                            break;
                        }

                    // RATE BASED CONFIGURATIONS
                    case RateBasedChargeConfiguration rateBasedChargeConfig when config is RateBasedChargeConfiguration && rateBasedChargeConfig.Rate != null:
                        {
                            calculator = new ReverseRateCalculator(rateBasedChargeConfig.Name, (oi) => rateBasedChargeConfig.Rate, rateBasedChargeConfig.BaseChargeNames);
                            break;
                        }
                    case RateBasedChargeConfiguration rateBasedChargeConfig when config is RateBasedChargeConfiguration && rateBasedChargeConfig.Name.Value is ChargeNames.Duty:
                        {
                            calculator = new ReverseRateCalculator(rateBasedChargeConfig.Name, (oi) => oi.DutyRate, rateBasedChargeConfig.BaseChargeNames);
                            break;
                        }
                    case RateBasedChargeConfiguration rateBasedChargeConfig when config is RateBasedChargeConfiguration && rateBasedChargeConfig.Name.Value is ChargeNames.Vat:
                        {
                            calculator = new ReverseRateCalculator(rateBasedChargeConfig.Name, (oi) => oi.VatRate, rateBasedChargeConfig.BaseChargeNames);
                            break;
                        }

                    default:
                        throw new InvalidChargeConfigurationException("Unknown charge configuration - can't create calculator delegate.");
                }

                // Add created calculator to delegate
                calculatorDelegate += calculator.Calculate;
            }

            // Add the item price calculator last
            calculatorDelegate += new ReverseItemPriceCalculator().Calculate;

            return calculatorDelegate;
        }
    }
}
