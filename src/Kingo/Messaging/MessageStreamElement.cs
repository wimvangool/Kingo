using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingo.Messaging
{
    internal abstract class MessageStreamElement : IEnumerable<IMessage>
    {
        public abstract void Accept(IMessageHandler visitor);

        public abstract IEnumerator<IMessage> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }             

        public abstract MessageStreamElement Append<TMessage>(TMessage message) where TMessage : class, IMessage;

        public abstract MessageStreamElement Append(MessageStreamElement element);

        internal abstract MessageStreamElement Insert(MessageStreamElement element);
    }
}
