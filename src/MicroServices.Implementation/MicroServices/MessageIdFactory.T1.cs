using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Kingo.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Kingo.MicroServices
{
    internal sealed class MessageIdFactory<TMessage> : IMessageIdFactory, IMessageIdFactory<TMessage>
    {
        private readonly IServiceProvider _serviceProvider;

        public MessageIdFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public string GenerateMessageIdFor(object message) =>
            GenerateMessageIdFor((TMessage) message);

        public string GenerateMessageIdFor(TMessage message)
        {
            var factories = _serviceProvider.GetServices<IMessageIdFactory<TMessage>>().ToArray();
            if (factories.Length == 0)
            {
                if (message is IMessage factory)
                {
                    return factory.GenerateMessageId();
                }
                return NewMessageId();
            }
            if (factories.Length == 1)
            {
                return factories[0].GenerateMessageIdFor(message);
            }
            throw NewMultipleFactoriesAddedForSameMessageTypeException(typeof(TMessage), factories.Select(factory => factory.GetType()));
        }

        private static string NewMessageId() =>
            Guid.NewGuid().ToString();

        private static Exception NewMultipleFactoriesAddedForSameMessageTypeException(Type messageType, IEnumerable<Type> factoryTypes)
        {
            var messageFormat = ExceptionMessages.MessageIdFactory_MultipleFactoriesAddedForSameMessageType;
            var message = string.Format(messageFormat, messageType.FriendlyName(), string.Join(", ", factoryTypes.Select(type => type.FriendlyName())));
            return new AmbiguousMatchException(message);
        }
    }
}
