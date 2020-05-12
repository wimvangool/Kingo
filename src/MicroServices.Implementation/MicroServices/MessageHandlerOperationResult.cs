using System;
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
            private readonly Message<object>[] _messages;

            public EmptyResult()
            {
                _messages = new Message<object>[0];
            }

            internal override IReadOnlyList<Message<object>> Messages =>
                _messages;

            public override int MessageHandlerCount =>
                0;

            internal override MessageHandlerOperationResult Append(MessageHandlerOperationResult result) =>
                result;

            internal override MessageHandlerOperationResult Commit(IMessage message, IServiceProvider serviceProvider) =>
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
        public IReadOnlyList<IMessage> Output =>
            Messages;

        internal abstract IReadOnlyList<Message<object>> Messages
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
            var messages = Messages.Concat(result.Messages);
            var messageHandlerCount = MessageHandlerCount + result.MessageHandlerCount;
            return new MessageListResult(messages, messageHandlerCount);
        }

        internal virtual MessageHandlerOperationResult Commit(IMessage message, IServiceProvider serviceProvider) =>
            new MessageListResult(Messages.Select(outputMessage => outputMessage.CorrelateWith(message).Validate(serviceProvider)), MessageHandlerCount);

        internal MessageHandlerOperationResult<TMessage> WithInput<TMessage>(IMessage<TMessage> input) =>
            new MessageHandlerOperationResult<TMessage>(this, input);
    }
}
