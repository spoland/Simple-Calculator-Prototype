using SimpleCalculator.Domain.Abstractions;
using SimpleCalculator.Domain.Calculators;
using SimpleCalculator.Domain.Calculators.Charge;
using SimpleCalculator.Domain.Calculators.Constraints;
using SimpleCalculator.Domain.Constants;
using SimpleCalculator.Domain.Entities;
using SimpleCalculator.Domain.Exceptions;
using SimpleCalculator.Domain.Models;
using SimpleCalculator.Domain.Models.ChargeConfigurations;

namespace SimpleCalculator.Domain.Factories
{
    public static class ForwardCalculatorFactory
    {
        public delegate void Calculator(Order order);

        public static Calculator? Create(CalculationRange range, ExcessConfiguration? excess)
        {
            Calculator? calculatorDelegate = null;
            ExcessCalculator? excessCalculator = null;

            // Excess is a special case and should execute first and last in the calculation pipeline.
            if (excess != null)
            {
                excessCalculator = new ExcessCalculator(excess);
                calculatorDelegate = excessCalculator.Calculate;
            }
            
            // Loop through each charge configuration (which are in order of execution) - and add the required
            // calculator to the delegate.
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
                        calculator = new RateBasedChargeCalculator(rateBasedChargeConfig.Name, (oi) => rateBasedChargeConfig.Rate, rateBasedChargeConfig.BaseChargeNames);
                        break;
                    }
                    case RateBasedChargeConfiguration rateBasedChargeConfig when config is RateBasedChargeConfiguration && rateBasedChargeConfig.Name.Value is ChargeNames.Duty:
                    {
                        calculator = new RateBasedChargeCalculator(rateBasedChargeConfig.Name, (oi) => oi.DutyRate, rateBasedChargeConfig.BaseChargeNames);
                        break;
                    }
                    case RateBasedChargeConfiguration rateBasedChargeConfig when config is RateBasedChargeConfiguration && rateBasedChargeConfig.Name.Value is ChargeNames.Vat:
                    {
                        calculator = new RateBasedChargeCalculator(rateBasedChargeConfig.Name, (oi) => oi.VatRate, rateBasedChargeConfig.BaseChargeNames);
                        break;
                    }

                    default:
                        throw new InvalidChargeConfigurationException("Unknown charge configuration - can't create calculator delegate.");
                }

                // Add constraints
                if (config.MinimumPayable != null) calculator = new MinimumPayableConstraint(calculator, config.Name, config.MinimumPayable);
                if (config.MinimumCollectible != null) calculator = new MinimumCollectibleConstraint(calculator, config.Name, config.MinimumCollectible);

                // Add created calculator to delegate
                calculatorDelegate += calculator.Calculate;
            }

            // If an excess calculator has been added, add it to the end to do clean up task
            if (excessCalculator != null) calculatorDelegate += excessCalculator.Calculate;

            return calculatorDelegate;
        }
    }
}
