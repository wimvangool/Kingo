using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Kingo.Messaging
{
    internal sealed class MessageStackTrace : IMessageStackTrace
    {
        private ImmutableStack<MessageInfo> _messages;

        public MessageStackTrace() :
            this(ImmutableStack<MessageInfo>.Empty) { }

        private MessageStackTrace(ImmutableStack<MessageInfo> messages)
        {
            _messages = messages;
        }

        public MessageStackTrace Copy() =>
            new MessageStackTrace(_messages);

        public MessageInfo Current =>
            _messages.IsEmpty ? null : _messages.Peek();

        public bool IsEmpty =>
            _messages.IsEmpty;

        public int Count =>
            _messages.Count();

        public MessageInfo this[int index] =>
            _messages.ElementAt(index);

        public IEnumerator<MessageInfo> GetEnumerator() =>
            _messages.AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();        

        public void Push(MessageInfo message) =>
            _messages = _messages.Push(message);

        public void Pop() =>
            _messages = _messages.Pop();

        public override string ToString() =>
            string.Join(" -> ", _messages.Reverse());
    }
}
