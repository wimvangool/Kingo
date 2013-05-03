
namespace YellowFlare.MessageProcessing.SampleHandlers.ForMessageHandlerClassTests
{    
    internal sealed class GenericCommandHandler<TMessage> : IExternalMessageHandler<TMessage>
        where TMessage : class
    {
        public void Handle(TMessage message) {}
    }
}
