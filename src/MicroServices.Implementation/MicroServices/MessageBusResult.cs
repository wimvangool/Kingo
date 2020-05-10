using System.Collections.Generic;
using System.Linq;

namespace Kingo.MicroServices
{
    internal sealed class MessageBusResult : MessageHandlerOperationResult
    {
        private readonly IReadOnlyList<Message<object>> _messages;

        public MessageBusResult(IReadOnlyList<Message<object>> messages)
        {
            _messages = messages;
        }

        internal override IReadOnlyList<Message<object>> Messages =>
            _messages;

        public override int MessageHandlerCount =>
            1;
    }
}
