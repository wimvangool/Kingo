using System;
using System.Collections.Concurrent;

namespace Kingo.MicroServices
{
    internal sealed class MessageIdFactory : IMessageIdFactory
    {
        private readonly ConcurrentDictionary<Type, IMessageIdFactory> _factoriesPerMessageType;
        private readonly IServiceProvider _serviceProvider;

        public MessageIdFactory(IServiceProvider serviceProvider)
        {
            _factoriesPerMessageType = new ConcurrentDictionary<Type, IMessageIdFactory>();
            _serviceProvider = serviceProvider;
        }

        public string GenerateMessageIdFor(object content) =>
            _factoriesPerMessageType.GetOrAdd(GetMessageType(content), ResolveMessageIdFactoryFor).GenerateMessageIdFor(content);

        private IMessageIdFactory ResolveMessageIdFactoryFor(Type messageType)
        {
            var factoryType = typeof(MessageIdFactory<>).MakeGenericType(messageType);
            var constructor = factoryType.GetConstructor(new[] { typeof(IServiceProvider) });
            return (IMessageIdFactory) constructor.Invoke(new object[] { _serviceProvider });
        }

        private static Type GetMessageType(object content) =>
            content?.GetType() ?? throw new ArgumentNullException(nameof(content));
    }
}
