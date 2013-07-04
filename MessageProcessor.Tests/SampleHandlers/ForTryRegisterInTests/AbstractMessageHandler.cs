
namespace YellowFlare.MessageProcessing.SampleHandlers.ForTryRegisterInTests
{    
    internal abstract class AbstractMessageHandler : IMessageHandler<Command>
    {
        public void Handle(Command message) {}
    }
}
