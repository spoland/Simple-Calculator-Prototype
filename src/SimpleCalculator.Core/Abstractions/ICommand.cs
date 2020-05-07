using Pricing.Advisor.Core.Abstractions;

namespace SimpleCalculator.Core.Abstractions
{
    /// <summary>
    /// Marker interface to represent a request with a response
    /// </summary>
    /// <typeparam name="TRequest">Request type</typeparam>
    /// <typeparam name="TResponse">Response type</typeparam>
    public interface ICommand<in TRequest, TResponse>
        where TRequest : IRequestMessage<TResponse>
    {
        /// <summary>
        /// Handle the request.
        /// </summary>
        /// <param name="message">The request message</param>
        /// <returns>A response of type <typeparamref name="TResponse"/></returns>
        TResponse Handle(TRequest message);
    }
}
