
namespace YellowFlare.MessageProcessing.SampleHandlers.ForMessageHandlerClassTests
{
    [InstanceLifetime((InstanceLifetime) 5)]
    internal sealed class MessageHandlerWithInvalidLifetimeAttribute : IExternalMessageHandler<Command>
    {
        public void Handle(Command message) {}            
    }
}
