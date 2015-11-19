using System.Threading.Tasks;

namespace Kingo.BuildingBlocks.Messaging.SampleHandlers.ForTryRegisterInTests
{
    [MessageHandler(InstanceLifetime.PerResolve)]
    internal sealed class MessageHandlerWithPerResolveLifetime : IMessageHandler<Command>
    {
        public Task HandleAsync(Command message)
        {
            return Task.Delay(0);
        }
    }
}
