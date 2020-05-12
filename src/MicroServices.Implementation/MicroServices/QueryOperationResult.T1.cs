using System;

namespace Kingo.MicroServices
{    
    /// <summary>
    /// Represents the result of executing a query by a <see cref="IMicroProcessor" />.
    /// </summary>
    /// <typeparam name="TResponse">Type of the returned response message.</typeparam>
    public class QueryOperationResult<TResponse> : IMicroProcessorOperationResult<TResponse>, IQueryOperationResult<TResponse>
    {
        private readonly Message<TResponse> _output;

        internal QueryOperationResult(Message<TResponse> output)
        {
            _output = output;
        }

        TResponse IMicroProcessorOperationResult<TResponse>.Value =>
            _output.Content;

        /// <inheritdoc />
        public IMessage<TResponse> Output =>
            _output;

        /// <inheritdoc />
        public override string ToString() =>
            _output.ToString();

        internal QueryOperationResult<TRequest, TResponse> WithInput<TRequest>(IMessage<TRequest> input) =>
            new QueryOperationResult<TRequest, TResponse>(_output, input);

        internal QueryOperationResult<TResponse> Commit(IMessage message, IServiceProvider serviceProvider) =>
            new QueryOperationResult<TResponse>(_output.CorrelateWith(message).Validate(serviceProvider));
    }
}
