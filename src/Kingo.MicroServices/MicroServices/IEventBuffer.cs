using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    internal interface IEventBuffer
    {
        Task<MessageHandlerOperationResult> HandleWith(IMessageProcessor processor, MessageHandlerOperationContext context);
    }
}
