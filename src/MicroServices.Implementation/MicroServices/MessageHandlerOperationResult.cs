using System.Collections.Generic;
using System.Linq;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents the result of a <see cref="IMessageHandler{TMessage}"/> operation executed by a <see cref="IMicroProcessor" />.
    /// </summary>
    public abstract class MessageHandlerOperationResult : IMicroProcessorOperationResult<IReadOnlyList<IMessage>>, IMessageHandlerOperationResult
    {
        #region [====== EmptyResult ======]

        private sealed class EmptyResult : MessageHandlerOperationResult
        {
            public EmptyResult()
            {
                Output = new IMessage[0];
            }

            public override IReadOnlyList<IMessage> Output
            {
                get;
            }

            public override int MessageHandlerCount =>
                0;

            internal override MessageHandlerOperationResult Append(MessageHandlerOperationResult result) =>
                result;

            internal override MessageHandlerOperationResult Commit(IMessage correlatedMessage) =>
                this;
        }

        #endregion

        /// <summary>
        /// Represents an empty result.
        /// </summary>
        public static readonly MessageHandlerOperationResult Empty = new EmptyResult();

        IReadOnlyList<IMessage> IMicroProcessorOperationResult<IReadOnlyList<IMessage>>.Value =>
            Output;

        /// <inheritdoc />
        public abstract IReadOnlyList<IMessage> Output
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
            $"{Output} ({nameof(MessageHandlerCount)} = {MessageHandlerCount})";

        internal virtual MessageHandlerOperationResult Append(MessageHandlerOperationResult result)
        {
            var messages = Output.Concat(result.Output);
            var messageHandlerCount = MessageHandlerCount + result.MessageHandlerCount;
            return new MessageListResult(messages, messageHandlerCount);
        }

        internal virtual MessageHandlerOperationResult Commit(IMessage correlatedMessage) =>
            new MessageListResult(Output.Select(message => message.CorrelateWith(correlatedMessage)), MessageHandlerCount);

        internal MessageHandlerOperationResult<TMessage> WithInput<TMessage>(Message<TMessage> input) =>
            new MessageHandlerOperationResult<TMessage>(this, input);
    }
}
