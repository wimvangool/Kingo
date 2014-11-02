
namespace System.ComponentModel.Server.SampleHandlers.ForTryRegisterInTests
{
    [InstanceLifetime(InstanceLifetime.PerResolve)]
    internal sealed class MessageHandlerWithPerResolveLifetime : IMessageHandler<Command>
    {
        public void Handle(Command message) {}
    }
}
