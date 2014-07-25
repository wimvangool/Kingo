
namespace System.ComponentModel.Messaging.Server.SampleHandlers.ForTryRegisterInTests
{    
    internal abstract class AbstractMessageHandler : IMessageHandler<Command>
    {
        public void Handle(Command message) {}
    }
}
