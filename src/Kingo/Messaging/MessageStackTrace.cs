using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Kingo.Messaging
{
    internal sealed class MessageStackTrace : IMessageStackTrace
    {
        private readonly MessageSources _defaultSource;
        private readonly Stack<MessageInfo> _messages;

        public MessageStackTrace(MessageSources defaultSource)
        {
            _defaultSource = defaultSource;
            _messages = new Stack<MessageInfo>();    
        }

        public MessageSources CurrentSource =>
            Current?.Source ?? _defaultSource;

        public MessageInfo Current =>
            IsEmpty ? null : _messages.Peek();

        public bool IsEmpty =>
            _messages.Count == 0;

        public int Count =>
            _messages.Count;

        public MessageInfo this[int index] =>
            _messages.ElementAt(index);

        public IEnumerator<MessageInfo> GetEnumerator() =>
            _messages.AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();        

        public void Push(MessageInfo message) =>
            _messages.Push(message);

        public void Pop() =>
            _messages.Pop();

        public override string ToString() =>
            string.Join(" -> ", _messages.Reverse());
    }
}
