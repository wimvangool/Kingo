using System.Collections.Generic;
using System.Linq;

namespace Kingo.MicroServices
{
    internal sealed class MessageListResult : MessageHandlerOperationResult
    {
        private readonly Message<object>[] _messages;
        private readonly int _messageHandlerCount;

        public MessageListResult(IEnumerable<Message<object>> messages, int messageHandlerCount)
        {
            _messages = messages.ToArray();
            _messageHandlerCount = messageHandlerCount;
        }

        internal override IReadOnlyList<Message<object>> Messages => 
            _messages;

        public override int MessageHandlerCount =>
            _messageHandlerCount;
    }
}
