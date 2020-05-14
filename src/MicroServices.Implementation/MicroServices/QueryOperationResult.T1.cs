namespace Kingo.MicroServices
{    
    /// <summary>
    /// Represents the result of executing a query by a <see cref="IMicroProcessor" />.
    /// </summary>
    /// <typeparam name="TResponse">Type of the returned response message.</typeparam>
    public sealed class QueryOperationResult<TResponse> : QueryOperationResultBase<IMessage, IMessage<TResponse>>
    {
        private readonly QueryOperationResult<VoidRequest, TResponse> _result;

        internal QueryOperationResult(QueryOperationResult<VoidRequest, TResponse> result)
        {
            _result = result;
        }

        /// <inheritdoc />
        public override IMessage Input =>
            _result.Input;

        /// <inheritdoc />
        public override IMessage<TResponse> Output =>
            _result.Output;
    }
}
