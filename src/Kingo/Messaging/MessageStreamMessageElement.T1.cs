using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingo.Messaging
{
    internal sealed class MessageStreamMessageElement<TMessage> : MessageStreamElement where TMessage : class, IMessage
    {
        private readonly TMessage _message;

        public MessageStreamMessageElement(TMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            _message = message;
        }

        public override void Accept(IMessageHandler visitor)
        {
            visitor.Handle(_message);
        }

        public override IEnumerator<IMessage> GetEnumerator()
        {
            yield return _message;
        }        

        public override MessageStreamElement Append<TMessage2>(TMessage2 message)
        {
            return new MessageStreamCompositeElement(this, new MessageStreamMessageElement<TMessage2>(message));
        }

        public override MessageStreamElement Append(MessageStreamElement element)
        {
            return element.Insert(this);
        }

        internal override MessageStreamElement Insert(MessageStreamElement element)
        {
            return new MessageStreamCompositeElement(element, this);
        }
    }
}
