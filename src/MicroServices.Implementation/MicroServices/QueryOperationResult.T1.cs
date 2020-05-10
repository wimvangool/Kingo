namespace Kingo.MicroServices
{    
    /// <summary>
    /// Represents the result of executing a query by a <see cref="IMicroProcessor" />.
    /// </summary>
    /// <typeparam name="TResponse">Type of the returned response message.</typeparam>
    public class QueryOperationResult<TResponse> : IMicroProcessorOperationResult<TResponse>, IQueryOperationResult<TResponse>
    {        
        internal QueryOperationResult(IMessage<TResponse> output)
        {
            Output = output;
        }

        TResponse IMicroProcessorOperationResult<TResponse>.Value =>
            Output.Content;

        /// <inheritdoc />
        public IMessage<TResponse> Output
        {
            get;
        }

        /// <inheritdoc />
        public override string ToString() =>
            Output == null ? "null" : Output.Content.ToString();

        internal QueryOperationResult<TRequest, TResponse> WithInput<TRequest>(IMessage<TRequest> input) =>
            new QueryOperationResult<TRequest, TResponse>(Output, input);
    }
}
