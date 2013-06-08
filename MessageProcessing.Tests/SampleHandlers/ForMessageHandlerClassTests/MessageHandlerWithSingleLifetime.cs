
namespace YellowFlare.MessageProcessing.SampleHandlers.ForMessageHandlerClassTests
{
    [InstanceLifetime(InstanceLifetime.Single)]
    internal sealed class MessageHandlerWithSingleLifetime : IMessageHandler<Command>
    {        
        public void Handle(Command message) {}  
    }
}
