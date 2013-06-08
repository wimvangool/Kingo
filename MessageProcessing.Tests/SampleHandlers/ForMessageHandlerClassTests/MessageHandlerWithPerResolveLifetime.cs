
namespace YellowFlare.MessageProcessing.SampleHandlers.ForMessageHandlerClassTests
{
    [InstanceLifetime(InstanceLifetime.PerResolve)]
    internal sealed class MessageHandlerWithPerResolveLifetime : IMessageHandler<Command>
    {
        public void Handle(Command message) {}
    }
}
