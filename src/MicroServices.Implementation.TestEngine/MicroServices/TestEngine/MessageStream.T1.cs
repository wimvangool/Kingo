using System;
using System.Collections.Generic;
using System.Linq;
using Kingo.Collections.Generic;
using Kingo.Reflection;

namespace Kingo.MicroServices.TestEngine
{
    /// <summary>
    /// Represents a set of messages of a specific type <typeparamref name="TMessage" />
    /// that were produced by a <see cref="IMicroProcessor"/> as a result of running a test.
    /// </summary>
    public sealed class MessageStream<TMessage> : ReadOnlyList<IMessage<TMessage>>
    {
        private readonly IMessage<TMessage>[] _messages;

        internal MessageStream(IEnumerable<IMessage<TMessage>> messages)
        {
            _messages = messages.ToArray();
        }

        #region [====== ToString ======]

        /// <inheritdoc />
        public override string ToString() =>
            MessageStream.ToString(this);

        #endregion

        #region [====== ReadOnlyList ======]

        /// <inheritdoc />
        public override int Count =>
            _messages.Length;

        /// <inheritdoc />
        public override IMessage<TMessage> this[int index] =>
            _messages[index];

        /// <inheritdoc />
        public override IEnumerator<IMessage<TMessage>> GetEnumerator() =>
            _messages.AsEnumerable().GetEnumerator();

        #endregion

        #region [====== AssertMessage ======]

        /// <summary>
        /// Asserts that this stream contains a message at the specified <paramref name="index"/>
        /// and its content satisfies the constraints specified in the <paramref name="assertion"/>.
        /// </summary>
        /// <param name="index">
        /// The relative index of the message in the stream. This index applies to the list of all messages
        /// of type <typeparamref name="TMessage"/> in the stream.
        /// </param>
        /// <param name="assertion">Delegate that will be used to assert the properties of the message.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="assertion" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="IndexOutOfRangeException">
        /// <paramref name="index"/> is negative.
        /// </exception>
        /// <exception cref="TestFailedException">
        /// This stream does not contain a message at the specified <paramref name="index"/> or the
        /// specified <paramref name="assertion"/> threw an exception.
        /// </exception>
        public void Assert(Action<TMessage> assertion, int index = 0) =>
            NotNull(assertion).Invoke(GetMessage(index).Content);

        /// <summary>
        /// Asserts that this stream contains a message at the specified <paramref name="index"/>
        /// and satisfies the constraints specified in the <paramref name="assertion"/>.
        /// </summary>
        /// <param name="index">
        /// The relative index of the message in the stream. This index applies to the list of all messages
        /// of type <typeparamref name="TMessage"/> in the stream.
        /// </param>
        /// <param name="assertion">Delegate that will be used to assert the properties of the message.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="assertion" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="IndexOutOfRangeException">
        /// <paramref name="index"/> is negative.
        /// </exception>
        /// <exception cref="TestFailedException">
        /// This stream does not contain a message at the specified <paramref name="index"/> or the
        /// specified <paramref name="assertion"/> threw an exception.
        /// </exception>
        public void AssertMessage(Action<IMessage<TMessage>> assertion, int index = 0) =>
            NotNull(assertion).Invoke(GetMessage(index));

        private static TDelegate NotNull<TDelegate>(TDelegate assertion) where TDelegate : class =>
            assertion ?? throw new ArgumentNullException(nameof(assertion));

        #endregion

        #region [====== GetMessage ======]

        /// <summary>
        /// Returns the message of the specified type <typeparamref name="TMessage"/>.
        /// </summary>
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
        public IMessage<TMessage> GetMessage(int index = 0)
        {
            if (index < 0)
            {
                throw NewIndexOutOfRangeException(index);
            }
            try
            {
                return _messages[index];
            }
            catch (IndexOutOfRangeException)
            {
                throw NewMessageOfTypeNotFoundException(index, _messages.Length);
            }
        }

        /// <summary>
        /// Attempts to return the message of the specified type <typeparamref name="TMessage"/>.
        /// </summary>
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
        public bool TryGetMessage(int index, out IMessage<TMessage> message)
        {
            if (index < 0)
            {
                throw NewIndexOutOfRangeException(index);
            }
            try
            {
                message = _messages[index];
                return true;
            }
            catch (IndexOutOfRangeException)
            {
                message = null;
                return false;
            }
        }

        private static Exception NewIndexOutOfRangeException(int index)
        {
            var messageFormat = ExceptionMessages.MessageStream_IndexOutOfRange;
            var message = string.Format(messageFormat, index);
            return new IndexOutOfRangeException(message);
        }

        private static Exception NewMessageOfTypeNotFoundException(int index, int messageCount)
        {
            var messageFormat = ExceptionMessages.MessageStream_MessageNotFound;
            var message = string.Format(messageFormat, typeof(TMessage).FriendlyName(), index, messageCount);
            return new TestFailedException(message);
        }

        #endregion
    }
}
