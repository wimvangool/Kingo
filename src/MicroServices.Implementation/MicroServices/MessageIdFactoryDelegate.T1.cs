using System;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    internal sealed class MessageIdFactoryDelegate<TMessage> : IMessageIdFactory<TMessage>
    {
        private readonly Func<TMessage, string> _factory;

        public MessageIdFactoryDelegate(Func<TMessage, string> factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public string GenerateMessageIdFor(TMessage message) =>
            _factory.Invoke(message);

        public override string ToString() =>
            GetType().FriendlyName();
    }
}
