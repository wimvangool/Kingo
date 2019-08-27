using System.Collections.Generic;

namespace Kingo.MicroServices
{    
    /// <summary>
    /// When implemented, represents the result of handling a command or event.
    /// </summary>
    public abstract class MessageHandlerOperationResult : IMicroProcessorOperationResult<IReadOnlyList<IMessage>>, IMessageHandlerOperationResult
    {                        
        IReadOnlyList<IMessage> IMicroProcessorOperationResult<IReadOnlyList<IMessage>>.Value =>
            Events;

        /// <inheritdoc />
        public abstract IReadOnlyList<IMessage> Events
        {
            get;
        }

        /// <inheritdoc />
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
