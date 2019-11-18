using System.Collections.Generic;
using System.Linq;

namespace Kingo.MicroServices
{
    internal sealed class MessageListResult : MessageHandlerOperationResult
    {
        private readonly MessageToDispatch[] _output;
        private readonly int _messageHandlerCount;

        public MessageListResult(IEnumerable<MessageToDispatch> output, int messageHandlerCount)
        {
            _output = output.ToArray();
            _messageHandlerCount = messageHandlerCount;
        }

        public override IReadOnlyList<MessageToDispatch> Output =>
            _output;

        public override int MessageHandlerCount =>
            _messageHandlerCount;
    }
}
