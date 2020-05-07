namespace Pricing.Advisor.Core.Abstractions
{
    /// <summary>
    /// A marker interface that represents a request message along with it's response type
    /// </summary>
    /// <typeparam name="TResponse">The type of response</typeparam>
    public interface IRequestMessage<TResponse>
    {
    }
}
