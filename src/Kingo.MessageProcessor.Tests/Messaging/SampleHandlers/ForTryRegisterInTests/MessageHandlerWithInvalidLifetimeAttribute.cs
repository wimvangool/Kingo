using System.Threading.Tasks;

namespace Kingo.Messaging.SampleHandlers.ForTryRegisterInTests
{
    [MessageHandler((MessageHandlerLifetime) 5)]
    internal sealed class MessageHandlerWithInvalidLifetimeAttribute : IMessageHandler<Command>
    {
        public Task HandleAsync(Command message)
        {
            return Task.Delay(0);
        }            
    }
}
