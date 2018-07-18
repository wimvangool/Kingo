namespace Kingo.Messaging
{    
    internal sealed class ExecuteAsyncResult<TMessageOut> : InvokeAsyncResult<TMessageOut>
    {                    
        public ExecuteAsyncResult(TMessageOut messageOut) :
            base(messageOut) { }

        /// <inheritdoc />
        public override string ToString() =>
            $"{(Value?.GetType() ?? typeof(TMessageOut)).FriendlyName()}";        
    }
}
