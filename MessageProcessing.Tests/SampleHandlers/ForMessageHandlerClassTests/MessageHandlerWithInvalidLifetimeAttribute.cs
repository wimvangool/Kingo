
namespace YellowFlare.MessageProcessing.SampleHandlers.ForMessageHandlerClassTests
{
    [InstanceLifetime((InstanceLifetime) 5)]
    internal sealed class MessageHandlerWithInvalidLifetimeAttribute : IMessageHandler<Command>
    {
        public void Handle(Command message) {}            
    }
}
