using System.Threading.Tasks;

namespace Kingo.Messaging.SampleHandlers.ForTryRegisterInTests
{
    [MessageHandler(MessageHandlerLifetime.PerUnitOfWork)]
    internal sealed class MessageHandlerWithPerUnitOfWorkLifetime : IMessageHandler<Command>
    {
        public Task HandleAsync(Command message)
        {
            return Task.Delay(0);
        }
    }
}
