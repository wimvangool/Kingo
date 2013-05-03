
namespace YellowFlare.MessageProcessing.SampleHandlers.ForMessageHandlerClassTests
{    
    internal abstract class AbstractMessageHandler : IExternalMessageHandler<Command>
    {
        public void Handle(Command message) {}
    }
}
