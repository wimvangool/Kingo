
using System.Threading.Tasks;

namespace System.ComponentModel.Server.SampleHandlers.ForTryRegisterInTests
{
    [MessageHandler(InstanceLifetime.Singleton)]
    internal sealed class MessageHandlerWithSingleLifetime : IMessageHandler<Command>
    {        
        public Task HandleAsync(Command message)
        {
            return Task.Delay(0);
        }  
    }
}
