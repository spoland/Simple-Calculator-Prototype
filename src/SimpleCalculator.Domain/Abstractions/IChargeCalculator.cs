using SimpleCalculator.Domain.Entities;

namespace SimpleCalculator.Domain.Abstractions
{
    public interface IChargeCalculator
    {
        void Calculate(Order order);
    }
}
