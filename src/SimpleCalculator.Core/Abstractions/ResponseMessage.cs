namespace Pricing.Advisor.Core.Abstractions
{
    /// <summary>
    /// Response message abstraction
    /// </summary>
    public abstract class ResponseMessage
    {
        /// <summary>
        /// Indicates if the request was successful
        /// </summary>
        public bool Success { get; private set; }

        /// <summary>
        /// Creates a new <see cref="ResponseMessage"/>
        /// </summary>
        /// <param name="success">A boolean indicatings if the request was successful</param>
        protected ResponseMessage(bool success)
        {
            Success = success;
        }
    }
}
