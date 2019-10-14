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

        #region [====== AssertMessage & GetMessage ======]

        /// <summary>
        /// Asserts that this stream contains a command at the specified <paramref name="index"/> and that
        /// this message is an instance of type <typeparamref name="TCommand"/>.
        /// </summary>
        /// <typeparam name="TCommand">Expected type of the command.</typeparam>
        /// <param name="index">Index of the message.</param>
        /// <param name="assertion">Optional delegate to verify the details of the message.</param>        
        /// <exception cref="IndexOutOfRangeException">
        /// <paramref name="index"/> is negative.
        /// </exception>
        /// <exception cref="TestFailedException">
        /// There is no message at the specified <paramref name="index"/>, or the message at that
        /// <paramref name="index"/> is not a command of type <typeparamref name="TCommand"/>.
        /// </exception>
        public void AssertCommand<TCommand>(int index, Action<MessageToDispatch<TCommand>> assertion = null) =>
            AssertMessage(index, MessageKind.Command, assertion);

        /// <summary>
        /// Asserts that this stream contains an event at the specified <paramref name="index"/> and that
        /// this message is an instance of type <typeparamref name="TEvent"/>.
        /// </summary>
        /// <typeparam name="TEvent">Expected type of the event.</typeparam>
        /// <param name="index">Index of the message.</param>
        /// <param name="assertion">Optional delegate to verify the details of the message.</param>        
        /// <exception cref="IndexOutOfRangeException">
        /// <paramref name="index"/> is negative.
        /// </exception>
        /// <exception cref="TestFailedException">
        /// There is no message at the specified <paramref name="index"/>, or the message at that
        /// <paramref name="index"/> is not an event of type <typeparamref name="TEvent"/>.
        /// </exception>
        public void AssertEvent<TEvent>(int index, Action<MessageToDispatch<TEvent>> assertion = null) =>
            AssertMessage(index, MessageKind.Event, assertion);

        private void AssertMessage<TMessage>(int index, MessageKind kind, Action<MessageToDispatch<TMessage>> assertion)
        {
            MessageToDispatch message;

            try
            {
                message = _messages[index];
            }
            catch (IndexOutOfRangeException exception)
            {
                if (index < 0)
                {
                    throw;
                }
                throw NewMessageNotFoundException(typeof(TMessage), index, Count, exception);
            }
            if (message.Kind != kind)
            {
                throw NewMessageNotOfExpectedKindException(kind, message.Kind, index);
            }
            if (message.IsOfType<TMessage>(out var messageOfExpectedType))
            {
                assertion?.Invoke(messageOfExpectedType);                
            }
            else
            {
                throw NewMessageNotOfExpectedTypeException(typeof(TMessage), index, message.GetType());
            }            
        }

        private static Exception NewMessageNotFoundException(Type expectedType, int index, int messageCount, Exception innerException)
        {
            var messageFormat = ExceptionMessages.MessageStream_MessageNotFound;
            var message = string.Format(messageFormat, expectedType.FriendlyName(), index, messageCount);
            return new TestFailedException(message, innerException);
        }

        private static Exception NewMessageNotOfExpectedKindException(MessageKind expectedKind, MessageKind actualKind, int index)
        {
            var messageFormat = ExceptionMessages.MessageStream_MessageNotOfExpectedKind;
            var message = string.Format(messageFormat, index, expectedKind, actualKind);
            return new TestFailedException(message);
        }

        private static Exception NewMessageNotOfExpectedTypeException(Type expectedType, int index, Type actualType)
        {
            var messageFormat = ExceptionMessages.MessageStream_MessageNotOfExpectedType;
            var message = string.Format(messageFormat, expectedType.FriendlyName(), index, actualType.FriendlyName());
            return new TestFailedException(message);
        }        

        /// <summary>
        /// Returns the message at the specified <paramref name="index" />.
        /// </summary>
        /// <typeparam name="TMessage">Expected message type.</typeparam>
        /// <param name="index">Index of the requested message.</param>
        /// <returns>The message at index <paramref name="index"/>.</returns>
        /// <exception cref="IndexOutOfRangeException">
        /// <paramref name="index"/> is not a valid index for this stream.
        /// </exception>
        /// <exception cref="InvalidCastException">
        /// The message at the specified <paramref name="index"/> is not of type <typeparamref name="TMessage"/>.
        /// </exception>
        protected TMessage GetMessage<TMessage>(int index) =>
            (TMessage) _messages[index].Content;

        #endregion
    }
}
