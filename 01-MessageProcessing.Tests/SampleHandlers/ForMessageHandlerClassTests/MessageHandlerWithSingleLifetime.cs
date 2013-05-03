
namespace YellowFlare.MessageProcessing.SampleHandlers.ForMessageHandlerClassTests
{
    [InstanceLifetime(InstanceLifetime.Single)]
    internal sealed class MessageHandlerWithSingleLifetime : IExternalMessageHandler<Command>
    {        
        public void Handle(Command message) {}  
    }
}
