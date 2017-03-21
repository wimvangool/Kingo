using System.Threading.Tasks;

namespace Kingo.Messaging.SampleHandlers.ForTryRegisterInTests
{
    [MessageHandler(MessageHandlerLifetime.PerResolve)]
    internal sealed class MessageHandlerWithPerResolveLifetime : IMessageHandler<Command>
    {
        public Task HandleAsync(Command message)
        {
            return Task.Delay(0);
        }
    }
}
