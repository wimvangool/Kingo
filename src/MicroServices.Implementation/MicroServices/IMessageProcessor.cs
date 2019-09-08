using System.Threading.Tasks;

namespace Kingo.MicroServices
{    
    internal interface IMessageProcessor
    {        
        Task<MessageHandlerOperationResult> HandleAsync<TMessage>(IMessageToProcess<TMessage> message, MessageHandlerOperationContext context);
    }
}
