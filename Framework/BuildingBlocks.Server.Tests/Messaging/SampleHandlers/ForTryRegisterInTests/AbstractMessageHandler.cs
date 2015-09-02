using System.Threading.Tasks;

namespace Kingo.BuildingBlocks.Messaging.SampleHandlers.ForTryRegisterInTests
{    
    internal abstract class AbstractMessageHandler : IMessageHandler<Command>
    {
        public Task HandleAsync(Command message)
        {
            return Task.Delay(0);
        }
    }
}
