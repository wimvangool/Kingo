using System.Collections.Generic;

namespace Kingo.MicroServices
{    
    /// <summary>
    /// When implemented, represents the result of handling a message by a <see cref="IMessageProcessor" />.
    /// </summary>
    public abstract class MessageHandlerOperationResult : IMicroProcessorOperationResult<IReadOnlyList<IMessage>>
    {                        
        IReadOnlyList<IMessage> IMicroProcessorOperationResult<IReadOnlyList<IMessage>>.Value =>
            Events;

        /// <summary>
        /// The events that were published during the operation.
        /// </summary>
        public abstract IReadOnlyList<IMessage> Events
        {
            get;
        }

        /// <summary>
        /// The number of message handlers that have handled the message.
        /// </summary>
        public abstract int MessageHandlerCount
        {
            get;
        }

        internal abstract EventBufferResult ToEventBufferResult();

        /// <inheritdoc />
        public override string ToString() =>
            $"{Events} ({nameof(MessageHandlerCount)} = {MessageHandlerCount})";        
    }
}
