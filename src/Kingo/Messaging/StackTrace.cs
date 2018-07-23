using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Kingo.Messaging
{
    internal sealed class StackTrace : IStackTrace
    {
        private readonly Stack<MicroProcessorOperation> _messages;

        public StackTrace()
        {
            _messages = new Stack<MicroProcessorOperation>();
        }

        public MicroProcessorOperation CurrentOperation =>
            IsEmpty ? null : _messages.Peek();

        public bool IsEmpty =>
            _messages.Count == 0;

        public int Count =>
            _messages.Count;

        public MicroProcessorOperation this[int index] =>
            _messages.ElementAt(index);

        public IEnumerator<MicroProcessorOperation> GetEnumerator() =>
            _messages.AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        public void Push(MicroProcessorOperationTypes operationType, object message) =>
            Push(new MicroProcessorOperation(operationType, message));

        public void Push(MicroProcessorOperation message) =>
            _messages.Push(message);

        public void Pop() =>
            _messages.Pop();

        public override string ToString() =>
            string.Join(" -> ", _messages.Reverse());
    }
}
