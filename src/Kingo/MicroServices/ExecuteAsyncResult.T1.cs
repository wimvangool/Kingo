namespace Kingo.MicroServices
{    
    internal sealed class ExecuteAsyncResult<TMessageOut> : InvokeAsyncResult<TMessageOut>
    {
        private readonly TMessageOut _messageOut;

        public ExecuteAsyncResult(TMessageOut messageOut)
        {
            _messageOut = messageOut;
        }

        public override TMessageOut GetValue() =>
            _messageOut;

        /// <inheritdoc />
        public override string ToString() =>
            _messageOut == null ? "null" : _messageOut.ToString();
    }
}
