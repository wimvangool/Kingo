namespace System.ComponentModel.Server
{    
    internal sealed class PassThroughPipeline<TMessage> : IMessageHandler<TMessage> where TMessage : class
    {
        private readonly IMessageHandler<TMessage> _handler;
        
        internal PassThroughPipeline(IMessageHandler<TMessage> handler)
        {            
            _handler = handler;
        }       
       
        public void Handle(TMessage message)
        {
            _handler.Handle(message);
        }
    }
}
