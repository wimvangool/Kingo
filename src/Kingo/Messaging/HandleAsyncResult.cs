namespace Kingo.Messaging
{    
    internal sealed class HandleAsyncResult : InvokeAsyncResult<IMessageStream>
    {                
        public HandleAsyncResult(IMessageStream outputStream) :
            base(outputStream) { }        

        /// <inheritdoc />
        public override string ToString() =>
            $"{Value.Count} event(s)";        
    }
}
