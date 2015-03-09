using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.ComponentModel.Server
{
    internal sealed class MessageToStrategyMapping<TStrategy> : IMessageToStrategyMapping<TStrategy> where TStrategy : class
    {
        public void Add(IMessage message, TStrategy strategy)
        {
            throw new NotImplementedException();
        }

        public void Add(Type messageType, TStrategy strategy)
        {
            throw new NotImplementedException();
        }

        public void Add(string messageTypeId, TStrategy strategy)
        {
            throw new NotImplementedException();
        }

        internal void Commit()
        {
            throw new NotImplementedException();
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
            strategy = null;
            return false;
        }
    }
}
