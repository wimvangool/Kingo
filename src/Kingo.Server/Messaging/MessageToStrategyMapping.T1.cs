using System;
using System.Collections.Generic;

namespace Kingo.Messaging
{
    internal sealed class MessageToStrategyMapping<TStrategy> : MessageToStrategyMapping, IMessageToStrategyMapping<TStrategy> where TStrategy : class
    {
        private readonly Dictionary<Type, TStrategy> _mapping;

        internal MessageToStrategyMapping()
        {
            _mapping = new Dictionary<Type, TStrategy>();
        }

        public void Add(IMessage message, TStrategy strategy)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            Add(message.GetType(), strategy);
        }

        public void Add(Type messageType, TStrategy strategy)
        {
            if (messageType == null)
            {
                throw new ArgumentNullException(nameof(messageType));
            }
            _mapping.Add(messageType, strategy);
        }              

        public bool TryGetStrategy(IMessage message, out TStrategy strategy)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            return TryGetStrategy(message.GetType(), out strategy);
        }

        public bool TryGetStrategy(Type messageType, out TStrategy strategy)
        {
            return _mapping.TryGetValue(messageType, out strategy);
        }        
    }
}
