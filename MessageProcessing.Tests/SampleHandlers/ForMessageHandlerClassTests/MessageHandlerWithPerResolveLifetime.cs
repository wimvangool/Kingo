
namespace YellowFlare.MessageProcessing.SampleHandlers.ForMessageHandlerClassTests
{
    [InstanceLifetime(InstanceLifetime.PerResolve)]
    internal sealed class MessageHandlerWithPerResolveLifetime : IExternalMessageHandler<Command>
    {
        public void Handle(Command message) {}
    }
}
