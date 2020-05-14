using System;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents the result of executing a query by a <see cref="IMicroProcessor" />.
    /// </summary>
    /// <typeparam name="TRequest">Type of the request-message.</typeparam>
    /// <typeparam name="TResponse">Type of the returned response message.</typeparam>
    public sealed class QueryOperationResult<TRequest, TResponse> : QueryOperationResultBase<IMessage<TRequest>, IMessage<TResponse>>
    {
        private readonly Message<TRequest> _input;
        private readonly Message<TResponse> _output;

        internal QueryOperationResult(Message<TRequest> input, Message<TResponse> output)
        {
            _input = input;
            _output = output.CorrelateWith(input);
        }

        /// <inheritdoc />
        public override IMessage<TRequest> Input =>
            _input;

        /// <inheritdoc />
        public override IMessage<TResponse> Output =>
            _output;

        internal QueryOperationResult<TRequest, TResponse> Validate(IServiceProvider serviceProvider) =>
            new QueryOperationResult<TRequest, TResponse>(_input, _output.Validate(serviceProvider));
    }
}
