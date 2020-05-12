using System;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents the result of executing a query by a <see cref="IMicroProcessor" />.
    /// </summary>
    /// <typeparam name="TRequest">Type of the request-message.</typeparam>
    /// <typeparam name="TResponse">Type of the returned response message.</typeparam>
    public sealed class QueryOperationResult<TRequest, TResponse> : QueryOperationResult<TResponse>
    {
        internal QueryOperationResult(Message<TResponse> output, IMessage<TRequest> input) : base(output)
        {
            Input = input;
        }

        /// <summary>
        /// The request-message of the query.
        /// </summary>
        public IMessage<TRequest> Input
        {
            get;
        }
    }
}
