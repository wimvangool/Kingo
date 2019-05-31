using System;

namespace Kingo.MicroServices
{    
    /// <summary>
    /// When implemented, represents the result of handling a message by a <see cref="IMessageProcessor" />.
    /// </summary>
    public abstract class HandleAsyncResult : IMicroProcessorOperationResult<MessageStream>
    {        
        /// <summary>
        /// Represents an empty result.
        /// </summary>
        public static readonly HandleAsyncResult Empty = new HandleAsyncMessageStreamResult(MessageStream.Empty, 0);

        internal HandleAsyncResult() { }        

        MessageStream IMicroProcessorOperationResult<MessageStream>.Value =>
            Events;

        /// <summary>
        /// The events that were published during the operation.
        /// </summary>
        public abstract MessageStream Events
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

        /// <summary>
        /// Merges this result with the specified <paramref name="result"/> and returns
        /// a new result containing both message streams and the aggregated message handler count.
        /// </summary>
        /// <param name="result">Another result.</param>
        /// <returns>The combined result.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="result"/> is <c>null</c>.
        /// </exception>
        public HandleAsyncResult Append(HandleAsyncResult result)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }
            return new HandleAsyncMessageStreamResult(Events.Concat(result.Events), MessageHandlerCount + result.MessageHandlerCount);
        }

        /// <inheritdoc />
        public override string ToString() =>
            $"{Events} ({nameof(MessageHandlerCount)} = {MessageHandlerCount})";        
    }
}
