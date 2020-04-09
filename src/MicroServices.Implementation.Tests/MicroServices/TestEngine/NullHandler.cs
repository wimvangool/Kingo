using System;
using System.Threading.Tasks;

namespace Kingo.MicroServices.TestEngine
{
    internal sealed class NullHandler : IMessageHandler<object>
    {
        public Task HandleAsync(object message, IMessageHandlerOperationContext context) =>
            Task.CompletedTask;
    }
}
