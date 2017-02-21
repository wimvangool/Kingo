using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingo.Messaging
{
    internal sealed class MessageStreamCompositeElement : MessageStreamElement
    {
        private readonly MessageStreamElement _leftElement;
        private readonly MessageStreamElement _rightElement;

        public MessageStreamCompositeElement(MessageStreamElement leftElement, MessageStreamElement rightElement)
        {
            _leftElement = leftElement;
            _rightElement = rightElement;
        }

        public override void Accept(IMessageHandler visitor)
        {
            _leftElement.Accept(visitor);
            _rightElement.Accept(visitor);
        }

        public override IEnumerator<IMessage> GetEnumerator()
        {
            return _leftElement.Concat(_rightElement).GetEnumerator();
        }

        public override MessageStreamElement Append<TMessage>(TMessage message)
        {
            return new MessageStreamCompositeElement(this, new MessageStreamMessageElement<TMessage>(message));
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
