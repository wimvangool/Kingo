using System.Threading.Tasks;

namespace Kingo.Messaging.SampleHandlers.ForTryRegisterInTests
{
    [MessageHandler(MessageHandlerLifetime.Singleton)]
    internal sealed class MessageHandlerWithSingleLifetime : IMessageHandler<Command>
    {        
        public Task HandleAsync(Command message)
        {
            return Task.Delay(0);
        }  
    }
}
