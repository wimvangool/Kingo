
using System.Threading.Tasks;

namespace Syztem.ComponentModel.Server.SampleHandlers.ForTryRegisterInTests
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
