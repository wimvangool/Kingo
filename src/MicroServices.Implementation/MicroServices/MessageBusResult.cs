using System.Collections.Generic;

namespace Kingo.MicroServices
{
    internal sealed class MessageBusResult : MessageHandlerOperationResult
    {
        public MessageBusResult(IReadOnlyList<MessageToDispatch> messages)
        {
            Messages = messages;
        }

        public override IReadOnlyList<MessageToDispatch> Messages
        {
            get;
        }

        public override int MessageHandlerCount =>
            1;

        internal override MessageBufferResult ToMessageBufferResult() =>
            new MessageBufferResult(new MessageBuffer(Messages), MessageHandlerCount);
    }
}
