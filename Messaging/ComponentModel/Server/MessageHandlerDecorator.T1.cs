namespace System.ComponentModel.Server
{    
    internal sealed class MessageHandlerDecorator<TMessage> : IMessageHandler<TMessage> where TMessage : class
    {
        private readonly IMessageHandler<TMessage> _handler;
        
        internal MessageHandlerDecorator(IMessageHandler<TMessage> handler)
        {            
            _handler = handler;
        }              
       
        public void Handle(TMessage message)
        {
            _handler.Handle(message);
        }
    }
}
