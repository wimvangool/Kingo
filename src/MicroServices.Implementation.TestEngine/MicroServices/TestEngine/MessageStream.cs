using System;
using System.Collections.Generic;
using System.Linq;
using Kingo.Collections.Generic;
using Kingo.Reflection;

namespace Kingo.MicroServices.TestEngine
{
    /// <summary>
    /// Represents a set of messages that were produced by a <see cref="IMicroProcessor"/> as a result of running a test.
    /// </summary>
    public sealed class MessageStream : ReadOnlyList<IMessage>
    {
        private readonly IMessage[] _messages;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageStream" /> class.
        /// </summary>
        /// <param name="messages">The messages of this stream.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messages"/> is <c>null</c>.
        /// </exception>
        public MessageStream(IEnumerable<IMessage> messages)
        {
            _messages = messages.ToArray();
        }

        #region [====== ToString ======]

        /// <inheritdoc />
        public override string ToString() =>
            ToString(this);

        internal static string ToString(IReadOnlyCollection<IMessage> messages) =>
            $"[{messages.Count}]" + ToTypeList(messages);

        private static string ToTypeList(IReadOnlyCollection<IMessage> messages) =>
            messages.Count == 0 ? string.Empty : " { " + ToTypeList(messages.Select(message => message.Content.GetType()), 3) + " }";

        private static string ToTypeList(IEnumerable<Type> messageTypes, int maxItems)
        {
            return string.Join(", ", messageTypes.Take(maxItems + 1).Select((messageType, index) =>
            {
                if (index < maxItems)
                {
                    return messageType.FriendlyName();
                }
                return "...";
            }));
        }

        #endregion

        #region [====== ReadOnlyList ======]

        /// <inheritdoc />
        public override IMessage this[int index] =>
            _messages[index];

        /// <inheritdoc />
        public override int Count =>
            _messages.Length;

        /// <inheritdoc />
        public override IEnumerator<IMessage> GetEnumerator() =>
            _messages.AsEnumerable().GetEnumerator();

        #endregion

        #region [====== AssertMessage ======]

        /// <summary>
        /// Asserts that this stream contains a message at the specified <paramref name="index"/>
        /// and its content satisfies the constraints specified in the <paramref name="assertion"/>.
        /// </summary>
        /// <typeparam name="TMessage">Type of the content to assert.</typeparam>
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
        public void Assert<TMessage>(Action<TMessage> assertion, int index = 0) =>
            GetMessages<TMessage>().Assert(assertion, index);

        /// <summary>
        /// Asserts that this stream contains a message at the specified <paramref name="index"/>
        /// and satisfies the constraints specified in the <paramref name="assertion"/>.
        /// </summary>
        /// <typeparam name="TMessage">Type of the content of the message to assert.</typeparam>
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
        public void AssertMessage<TMessage>(Action<Message<TMessage>> assertion, int index = 0) =>
            GetMessages<TMessage>().AssertMessage(assertion, index);

        #endregion

        #region [====== GetMessage ======]

        /// <summary>
        /// Returns the message of the specified type <typeparamref name="TMessage"/>.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message to return.</typeparam>
        /// <param name="index">
        /// Indicates which message must be returned if this stream contains
        /// multiple messages of type <typeparamref name="TMessage"/>.
        /// </param>
        /// <returns>A message of the selected type.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is negative.
        /// </exception>
        /// <exception cref="TestFailedException">
        /// The requested message identified by the specified <paramref name="index"/> was not found.
        /// </exception>
        public Message<TMessage> GetMessage<TMessage>(int index = 0) =>
            GetMessages<TMessage>().GetMessage(index);

        /// <summary>
        /// Attempts to return the message of the specified type <typeparamref name="TMessage"/>.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message to return.</typeparam>
        /// <param name="index">
        /// Indicates which message must be returned if this stream contains
        /// multiple messages of type <typeparamref name="TMessage"/>.
        /// </param>
        /// <param name="message">
        /// If the message of the specified type <typeparamref name="TMessage"/> was found at the specified
        /// <paramref name="index"/>, this parameter will be assigned to the requested message; otherwise it
        /// will be <c>null</c>.
        /// </param>
        /// <returns><c>true</c> if the messages was found; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is negative.
        /// </exception>
        public bool TryGetMessage<TMessage>(int index, out Message<TMessage> message) =>
            GetMessages<TMessage>().TryGetMessage(index, out message);

        /// <summary>
        /// Returns all messages in this stream of the specified type <typeparamref name="TMessage"/>.
        /// </summary>
        /// <typeparam name="TMessage">Type of the messages to return.</typeparam>
        /// <returns>A new stream containing all messages of type <typeparamref name="TMessage"/>.</returns>
        public MessageStream<TMessage> GetMessages<TMessage>() =>
            new MessageStream<TMessage>(GetMessagesOfType<TMessage>());

        private IEnumerable<Message<TMessage>> GetMessagesOfType<TMessage>()
        {
            foreach (var message in _messages)
            {
                if (message.TryConvertTo<TMessage>(out var messageOfRequestedType))
                {
                    yield return messageOfRequestedType;
                }
            }
        }

        #endregion
    }
}
