using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingo.Messaging
{
    internal sealed class MessageStreamNullElement : MessageStreamElement
    {
        public override void Accept(IMessageHandler visitor) { }

        public override IEnumerator<IMessage> GetEnumerator()
        {
            return Enumerable.Empty<IMessage>().GetEnumerator();
        }        

        public override MessageStreamElement Append<TMessage>(TMessage message)
        {
            return new MessageStreamMessageElement<TMessage>(message);
        }

        public override MessageStreamElement Append(MessageStreamElement element)
        {
            return element;
        }

        internal override MessageStreamElement Insert(MessageStreamElement element)
        {
            return element;
        }
    }
}
