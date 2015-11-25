using System.Threading.Tasks;

namespace Kingo.Messaging
{    
    internal sealed class MessageHandlerDecorator<TMessage> : IMessageHandler<TMessage> where TMessage : class
    {
        private readonly IMessageHandler<TMessage> _handler;
        
        internal MessageHandlerDecorator(IMessageHandler<TMessage> handler)
        {            
            _handler = handler;
        }              
       
        public Task HandleAsync(TMessage message)
        {
            return _handler.HandleAsync(message);
        }
    }
}
