using System.Collections.Generic;
using System.Linq;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents the result of a <see cref="IMessageHandler{TMessage}"/> operation executed by a <see cref="IMicroProcessor" />.
    /// </summary>
    public abstract class MessageHandlerOperationResult : IMicroProcessorOperationResult<IReadOnlyList<MessageToDispatch>>, IMessageHandlerOperationResult
    {
        #region [====== EmptyResult ======]

        private sealed class EmptyResult : MessageHandlerOperationResult
        {
            public EmptyResult()
            {
                Messages = new MessageToDispatch[0];
            }

            public override IReadOnlyList<MessageToDispatch> Messages
            {
                get;
            }

            public override int MessageHandlerCount =>
                0;

            internal override MessageHandlerOperationResult Append(MessageHandlerOperationResult result) =>
                result;

            internal override MessageHandlerOperationResult Commit(IMessageEnvelope correlatedMessage) =>
                this;
        }

        #endregion

        public static readonly MessageHandlerOperationResult Empty = new EmptyResult();

        IReadOnlyList<MessageToDispatch> IMicroProcessorOperationResult<IReadOnlyList<MessageToDispatch>>.Value =>
            Messages;

        /// <inheritdoc />
        public abstract IReadOnlyList<MessageToDispatch> Messages
        {
            get;
        }

        /// <inheritdoc />
        public abstract int MessageHandlerCount
        {
            get;
        }

        /// <inheritdoc />
        public override string ToString() =>
            $"{Messages} ({nameof(MessageHandlerCount)} = {MessageHandlerCount})";

        internal virtual MessageHandlerOperationResult Append(MessageHandlerOperationResult result)
        {
            var messages = Messages.Concat(result.Messages);
            var messageHandlerCount = MessageHandlerCount + result.MessageHandlerCount;
            return new MessageListResult(messages, messageHandlerCount);
        }

        internal virtual MessageHandlerOperationResult Commit(IMessageEnvelope correlatedMessage) =>
            new MessageListResult(Messages.Select(message => message.CorrelateWith(correlatedMessage)), MessageHandlerCount);
    }
}
