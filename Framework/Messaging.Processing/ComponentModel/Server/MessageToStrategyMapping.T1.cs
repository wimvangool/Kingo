using System;
using System.Collections.Generic;

namespace Syztem.ComponentModel.Server
{
    internal sealed class MessageToStrategyMapping<TStrategy> : MessageToStrategyMapping, IMessageToStrategyMapping<TStrategy> where TStrategy : class
    {
        private readonly Dictionary<string, TStrategy> _mapping;

        internal MessageToStrategyMapping()
        {
            _mapping = new Dictionary<string, TStrategy>();
        }

        public void Add(IMessage message, TStrategy strategy)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            Add(message.TypeId, strategy);
        }

        public void Add(Type messageType, TStrategy strategy)
        {
            Add(Message.TypeIdOf(messageType), strategy);
        }

        public void Add(string messageTypeId, TStrategy strategy)
        {
            if (messageTypeId == null)
            {
                throw new ArgumentNullException("messageTypeId");
            }
            _mapping.Add(messageTypeId, strategy);
        }        

        public bool TryGetStrategy(IMessage message, out TStrategy strategy)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            return TryGetStrategy(message.TypeId, out strategy);
        }

        public bool TryGetStrategy(Type messageType, out TStrategy strategy)
        {
            return TryGetStrategy(Message.TypeIdOf(messageType), out strategy);
        }

        public bool TryGetStrategy(string messageTypeId, out TStrategy strategy)
        {
            if (messageTypeId == null)
            {
                throw new ArgumentNullException("messageTypeId");
            }
            return _mapping.TryGetValue(messageTypeId, out strategy);
        }
    }
}
