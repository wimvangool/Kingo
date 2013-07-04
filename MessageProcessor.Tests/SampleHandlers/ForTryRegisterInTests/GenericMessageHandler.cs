
namespace YellowFlare.MessageProcessing.SampleHandlers.ForTryRegisterInTests
{    
    internal sealed class GenericCommandHandler<TMessage> : IMessageHandler<TMessage> where TMessage : class
    {
        public void Handle(TMessage message) {}
    }
}
