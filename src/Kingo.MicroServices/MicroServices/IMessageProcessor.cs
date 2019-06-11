using System.Threading.Tasks;

namespace Kingo.MicroServices
{    
    internal interface IMessageProcessor
    {        
        Task<MessageHandlerOperationResult> HandleAsync<TMessage>(IMessage<TMessage> message, MessageHandlerOperationContext context);
    }
}
