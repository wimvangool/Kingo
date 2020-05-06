using System.Collections.Generic;
using System.Linq;

namespace Kingo.MicroServices
{
    internal sealed class MessageListResult : MessageHandlerOperationResult
    {
        private readonly IMessage[] _output;
        private readonly int _messageHandlerCount;

        public MessageListResult(IEnumerable<IMessage> output, int messageHandlerCount)
        {
            _output = output.ToArray();
            _messageHandlerCount = messageHandlerCount;
        }

        public override IReadOnlyList<IMessage> Output =>
            _output;

        public override int MessageHandlerCount =>
            _messageHandlerCount;
    }
}
