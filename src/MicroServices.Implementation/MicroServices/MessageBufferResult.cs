using System.Collections.Generic;

namespace Kingo.MicroServices
{
    internal sealed class MessageBufferResult : MessageHandlerOperationResult
    {
        public static readonly MessageBufferResult Empty = new MessageBufferResult(MessageBuffer.Empty, 0);        

        public MessageBufferResult(MessageBuffer messageBuffer, int messageHandlerCount)
        {
            MessageBuffer = messageBuffer;
            MessageHandlerCount = messageHandlerCount;
        }

        public override IReadOnlyList<MessageToDispatch> Messages =>
            MessageBuffer;

        internal MessageBuffer MessageBuffer
        {
            get;
        }

        public override int MessageHandlerCount
        {
            get;
        }

        public MessageBufferResult Append(MessageHandlerOperationResult result) =>
            Append(result.ToMessageBufferResult());

        private MessageBufferResult Append(MessageBufferResult result)
        {
            var messageBuffer = MessageBuffer.Append(result.MessageBuffer);
            var messageHandlerCount = MessageHandlerCount + result.MessageHandlerCount;
            return new MessageBufferResult(messageBuffer, messageHandlerCount);
        }            

        internal override MessageBufferResult ToMessageBufferResult() =>
            this;
    }
}
