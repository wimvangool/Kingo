namespace Kingo.MicroServices
{    
    /// <summary>
    /// Represents the result of executing a query by a <see cref="IMicroProcessor" />.
    /// </summary>
    /// <typeparam name="TResponse">Type of the returned response message.</typeparam>
    public class QueryOperationResult<TResponse> : IMicroProcessorOperationResult<TResponse>, IQueryOperationResult<TResponse>
    {        
        internal QueryOperationResult(MessageEnvelope<TResponse> output)
        {
            Output = output;
        }

        TResponse IMicroProcessorOperationResult<TResponse>.Value =>
            Output.Content;
        
        /// <summary>
        /// The response that was returned by the query.
        /// </summary>
        public MessageEnvelope<TResponse> Output
        {
            get;
        }

        /// <inheritdoc />
        public override string ToString() =>
            Output == null ? "null" : Output.Content.ToString();

        internal QueryOperationResult<TRequest, TResponse> WithInput<TRequest>(MessageEnvelope<TRequest> input) =>
            new QueryOperationResult<TRequest, TResponse>(Output, input);
    }
}
