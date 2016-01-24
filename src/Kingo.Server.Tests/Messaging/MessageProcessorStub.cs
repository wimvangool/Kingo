namespace Kingo.Messaging
{
    internal sealed class MessageProcessorStub : MessageProcessor
    {
        protected override MessageHandlerFactory CreateMessageHandlerFactory(LayerConfiguration layers)
        {
            return new UnityFactory();
        }
    }
}
