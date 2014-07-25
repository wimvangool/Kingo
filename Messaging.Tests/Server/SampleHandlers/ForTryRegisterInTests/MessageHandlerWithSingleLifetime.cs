
namespace System.ComponentModel.Messaging.Server.SampleHandlers.ForTryRegisterInTests
{
    [InstanceLifetime(InstanceLifetime.Single)]
    internal sealed class MessageHandlerWithSingleLifetime : IMessageHandler<Command>
    {        
        public void Handle(Command message) {}  
    }
}
