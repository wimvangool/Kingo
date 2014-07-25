
namespace System.ComponentModel.Messaging.Server.SampleHandlers.ForTryRegisterInTests
{
    [InstanceLifetime(InstanceLifetime.PerResolve)]
    internal sealed class MessageHandlerWithPerResolveLifetime : IMessageHandler<Command>
    {
        public void Handle(Command message) {}
    }
}
