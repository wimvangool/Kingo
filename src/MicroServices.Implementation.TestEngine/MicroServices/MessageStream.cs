using System;
using System.Collections.Generic;
using System.Linq;
using Kingo.Collections.Generic;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents a set of messages that were produced by a <see cref="IMicroProcessor"/> as a result of running
    /// a test.
    /// </summary>
    public class MessageStream : ReadOnlyList<MessageToDispatch>
    {
        /// <summary>
        /// Represents an empty message-stream.
        /// </summary>
        public static readonly MessageStream Empty = new MessageStream(Enumerable.Empty<MessageToDispatch>());

        private readonly MessageToDispatch[] _messages;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageStream" /> class.
        /// </summary>
        /// <param name="stream">The stream to copy.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream"/> is <c>null</c>.
        /// </exception>
        protected MessageStream(MessageStream stream) :
            this(stream?._messages) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageStream" /> class.
        /// </summary>
        /// <param name="messages">The messages of this stream.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messages"/> is <c>null</c>.
        /// </exception>
        public MessageStream(IEnumerable<MessageToDispatch> messages)
        {
            _messages = messages.ToArray();
        }

        #region [====== ReadOnlyList ======]

        /// <inheritdoc />
        public override MessageToDispatch this[int index] =>
            _messages[index];

        /// <inheritdoc />
        public override int Count =>
            _messages.Length;

        /// <inheritdoc />
        public override IEnumerator<MessageToDispatch> GetEnumerator() =>
            _messages.AsEnumerable().GetEnumerator();

        #endregion

        #region [====== GetMessage ======]

        /// <summary>
        /// Returns the message of the specified type <typeparamref name="TMessage"/>.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message to return.</typeparam>
        /// <param name="index">
        /// Indicates which message must be returned if this stream contains multiple messages of type <typeparamref name="TMessage"/>.
        /// </param>
        /// <returns>A message of the selected type.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is negative.
        /// </exception>
        /// <exception cref="TestFailedException">
        /// The requested message identified by the specified <paramref name="index"/> was not found.
        /// </exception>
        protected MessageToDispatch<TMessage> GetMessage<TMessage>(int index = 0)
        {
            if (index < 0)
            {
                throw NewIndexOutOfRangeException(index);
            }
            var messages = GetMessages<TMessage>().ToArray();
            
            try
            {
                return messages[index];
            }
            catch (IndexOutOfRangeException)
            {
                throw NewMessageOfTypeNotFoundException(typeof(TMessage), index, messages.Length);
            }
        }

        /// <summary>
        /// Attempts to return the message of the specified type <typeparamref name="TMessage"/>.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message to return.</typeparam>
        /// <param name="index">
        /// Indicates which message must be returned if this stream contains multiple messages of type <typeparamref name="TMessage"/>.
        /// </param>
        /// <param name="message">
        /// If the message of the specified type <typeparamref name="TMessage"/> was found at the specified <paramref name="index"/>,
        /// this parameter will be assigned to the requested message; otherwise it will be <c>null</c>.
        /// </param>
        /// <returns><c>true</c> if the messages was found; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is negative.
        /// </exception>
        protected bool TryGetMessage<TMessage>(int index, out MessageToDispatch<TMessage> message)
        {
            if (index < 0)
            {
                throw NewIndexOutOfRangeException(index);
            }
            var messages = GetMessages<TMessage>().ToArray();

            try
            {
                message = messages[index];
                return true;
            }
            catch (IndexOutOfRangeException)
            {
                message = null;
                return false;
            }
        }

        /// <summary>
        /// Returns all messages in this stream of the specified type <typeparamref name="TMessage"/>.
        /// </summary>
        /// <typeparam name="TMessage">Type of the messages to return.</typeparam>
        /// <returns>A collection of messages.</returns>
        protected IEnumerable<MessageToDispatch<TMessage>> GetMessages<TMessage>()
        {
            foreach (var message in _messages)
            {
                if (message.IsOfType<TMessage>(out var messageOfRequestedType))
                {
                    yield return messageOfRequestedType;
                }
            }
        }

        private static Exception NewIndexOutOfRangeException(int index)
        {
            var messageFormat = ExceptionMessages.MessageStream_IndexOutOfRange;
            var message = string.Format(messageFormat, index);
            return new IndexOutOfRangeException(message);
        }

        private static Exception NewMessageOfTypeNotFoundException(Type messageType, int index, int messageCount)
        {
            var messageFormat = ExceptionMessages.MessageStream_MessageNotFound;
            var message = string.Format(messageFormat, messageType.FriendlyName(), index, messageCount);
            return new TestFailedException(message);
        }

        #endregion
    }
}
