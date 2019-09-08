using System.Collections.Generic;

namespace Kingo.MicroServices
{    
    /// <summary>
    /// When implemented, represents the result of handling a command or event.
    /// </summary>
    public abstract class MessageHandlerOperationResult : IMicroProcessorOperationResult<IReadOnlyList<MessageToDispatch>>, IMessageHandlerOperationResult
    {                        
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

        internal abstract MessageBufferResult ToMessageBufferResult();

        /// <inheritdoc />
        public override string ToString() =>
            $"{Messages} ({nameof(MessageHandlerCount)} = {MessageHandlerCount})";        
    }
}
