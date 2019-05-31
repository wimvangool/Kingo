using System.Collections.Generic;
using System.Linq;

namespace Kingo.MicroServices
{        
    internal sealed class MessageHandlerFactory : IMessageHandlerFactory
    {                
        public static readonly IMessageHandlerFactory Null = new MessageHandlerFactory(Enumerable.Empty<IMessageHandlerFactory>());

        private readonly IMessageHandlerFactory[] _messageHandlerFactories;        
              
        public MessageHandlerFactory(IEnumerable<IMessageHandlerFactory> messageHandlerFactories)
        {
            _messageHandlerFactories = messageHandlerFactories.ToArray();            
        }                
        
        public override string ToString() =>
            $"{_messageHandlerFactories.Length} MessageHandler(s) Registered";        
        
        public IEnumerable<MessageHandler> ResolveMessageHandlers<TMessage>(TMessage message, MessageHandlerContext context) =>
            from handlerClass in _messageHandlerFactories
            from handler in handlerClass.ResolveMessageHandlers(message, context)
            select handler;          
    }
}
