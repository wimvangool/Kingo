
using System.Threading.Tasks;

namespace System.ComponentModel.Server.SampleHandlers.ForTryRegisterInTests
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
