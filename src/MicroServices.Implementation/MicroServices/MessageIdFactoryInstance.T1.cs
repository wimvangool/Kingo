using Kingo.Reflection;

namespace Kingo.MicroServices
{
    internal sealed class MessageIdFactoryInstance<TMessage> : MessageIdFactoryComponent, IMessageIdFactory<TMessage>
    {
        private readonly IMessageIdFactory<TMessage> _factory;

        public MessageIdFactoryInstance(IMessageIdFactory<TMessage> factory) :
            base(MessageIdFactoryType.FromInstance(factory))
        {
            _factory = factory;
        }

        public string GenerateMessageIdFor(TMessage content) =>
            _factory.GenerateMessageIdFor(content);

        public override string ToString() =>
            _factory.GetType().FriendlyName();
    }
}
