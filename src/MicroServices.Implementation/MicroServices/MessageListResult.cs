using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kingo.MicroServices
{
    internal sealed class MessageListResult : MessageHandlerOperationResult
    {
        private readonly MessageToDispatch[] _messages;
        private readonly int _messageHandlerCount;

        public MessageListResult(IEnumerable<MessageToDispatch> messages, int messageHandlerCount)
        {
            _messages = messages.ToArray();
            _messageHandlerCount = messageHandlerCount;
        }

        public override IReadOnlyList<MessageToDispatch> Messages =>
            _messages;

        public override int MessageHandlerCount =>
            _messageHandlerCount;
    }
}
