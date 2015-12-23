namespace Kingo.Messaging
{
    internal sealed class MessageProcessorStub : MessageProcessor
    {
        protected override MessageHandlerFactory CreateMessageHandlerFactory()
        {
            return new UnityFactory();
        }
    }
}
