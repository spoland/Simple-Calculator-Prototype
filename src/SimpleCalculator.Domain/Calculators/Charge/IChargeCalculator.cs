using SimpleCalculator.Domain.Models;

namespace SimpleCalculator.Domain.Calculators
{
    public interface IChargeCalculator
    {
        void Calculate(Order order);
    }
}
