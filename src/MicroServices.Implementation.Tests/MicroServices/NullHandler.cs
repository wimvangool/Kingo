using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    internal sealed class NullHandler : IMessageHandler<object>
    {
        public Task HandleAsync(object message, MessageHandlerOperationContext context) =>
            Task.CompletedTask;
    }
}
