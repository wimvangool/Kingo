using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents the result of a <see cref="IMessageHandler{TMessage}"/> operation executed by a <see cref="IMicroProcessor" />.
    /// </summary>
    public abstract class MessageHandlerOperationResult : IReadOnlyCollection<Message<object>>
    {
        #region [====== IReadOnlyCollection<Message<object>> ======]

        int IReadOnlyCollection<Message<object>>.Count =>
            Output.Count;

        IEnumerator<Message<object>> IEnumerable<Message<object>>.GetEnumerator() =>
            GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        internal abstract IEnumerator<Message<object>> GetEnumerator();

        #endregion

        #region [====== Input & Output ======]

        /// <summary>
        /// Gets the list of messages that were produced by the operation.
        /// </summary>
        public abstract IReadOnlyList<IMessage> Output
        {
            get;
        }

        /// <summary>
        /// Gets the number of message-handlers that were invoked during the operation.
        /// </summary>
        public abstract int MessageHandlerCount
        {
            get;
        }

        #endregion

        #region [====== Append ======]

        internal static MessageHandlerOperationResult<TMessage> FromInput<TMessage>(Message<TMessage> input) =>
            new MessageHandlerOperationResult<TMessage>(input, Enumerable.Empty<Message<object>>());

        internal abstract MessageHandlerOperationResult<TMessage> AppendTo<TMessage>(MessageHandlerOperationResult<TMessage> result);

        #endregion
    }
}
