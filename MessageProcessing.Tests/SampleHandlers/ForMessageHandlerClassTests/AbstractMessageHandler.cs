
namespace YellowFlare.MessageProcessing.SampleHandlers.ForMessageHandlerClassTests
{    
    internal abstract class AbstractMessageHandler : IMessageHandler<Command>
    {
        public void Handle(Command message) {}
    }
}
