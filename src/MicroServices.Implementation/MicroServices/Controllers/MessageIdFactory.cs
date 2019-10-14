using System;
using System.Collections.Generic;

namespace Kingo.MicroServices.Controllers
{
    internal sealed class MessageIdFactory : IMessageIdFactory
    {
        private readonly Dictionary<Type, IMessageIdFactory> _factoriesPerMessageType;

        public MessageIdFactory(IDictionary<Type, IMessageIdFactory> factoriesPerMessageType)
        {
            _factoriesPerMessageType = new Dictionary<Type, IMessageIdFactory>(factoriesPerMessageType);
        }

        public string GenerateMessageIdFor(object content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }
            if (_factoriesPerMessageType.TryGetValue(content.GetType(), out var factory))
            {
                return factory.GenerateMessageIdFor(content);
            }
            return NewMessageId();
        }

        internal static string NewMessageId() =>
            Guid.NewGuid().ToString();
    }
}
