using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    internal interface IMessageBuffer
    {
        Task<MessageHandlerOperationResult> HandleEventsWith(IMessageProcessor processor, MessageHandlerOperationContext context);
    }
}
