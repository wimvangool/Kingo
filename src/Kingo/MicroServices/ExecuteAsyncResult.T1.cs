namespace Kingo.MicroServices
{    
    internal sealed class ExecuteAsyncResult<TResponse> : InvokeAsyncResult<TResponse>
    {
        private readonly TResponse _messageOut;

        public ExecuteAsyncResult(TResponse messageOut)
        {
            _messageOut = messageOut;
        }

        public override TResponse GetValue() =>
            _messageOut;

        /// <inheritdoc />
        public override string ToString() =>
            _messageOut == null ? "null" : _messageOut.ToString();
    }
}
