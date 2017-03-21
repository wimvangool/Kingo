using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Threading;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents a stream that contains a single message.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message contained by the stream.</typeparam>
    public sealed class MessageStream<TMessage> : ReadOnlyList<object>, IMessageStream
    {
        private readonly TMessage _message;
        private readonly IMessageHandler<TMessage> _handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageStream{T}" /> class.
        /// </summary>
        /// <param name="message">A message.</param>
        /// <param name="handler">Optional handler associated to this message.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        public MessageStream(TMessage message, Action<TMessage, IMicroProcessorContext> handler) :
            this(message, MessageHandler<TMessage>.FromDelegate(handler)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageStream{T}" /> class.
        /// </summary>
        /// <param name="message">A message.</param>
        /// <param name="handler">Optional handler associated to this message.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        public MessageStream(TMessage message, Func<TMessage, IMicroProcessorContext, Task> handler) :
            this(message, MessageHandler<TMessage>.FromDelegate(handler)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageStream{T}" /> class.
        /// </summary>
        /// <param name="message">A message.</param>
        /// <param name="handler">Optional handler associated to this message.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        public MessageStream(TMessage message, IMessageHandler<TMessage> handler = null)
        {
            if (ReferenceEquals(message, null))
            {
                throw new ArgumentNullException(nameof(message));
            }
            _message = message;
            _handler = handler;
        }

        #region [====== IReadOnlyList<object> ======]

        /// <inheritdoc />
        public override int Count => 1;

        /// <inheritdoc />
        public override IEnumerator<object> GetEnumerator()
        {
            yield return _message;
        }        

        #endregion

        #region [====== IMessageStream ======]        

        /// <inheritdoc />
        public IMessageStream Append<TOther>(TOther message, Action<TOther, IMicroProcessorContext> handler = null) =>
            new MessageStream(this, new MessageStream<TOther>(message, handler));

        /// <inheritdoc />
        public IMessageStream Append<TOther>(TOther message, Func<TOther, IMicroProcessorContext, Task> handler = null) =>
            new MessageStream(this, new MessageStream<TOther>(message, handler));

        /// <inheritdoc />
        public IMessageStream Append<TOther>(TOther message, IMessageHandler<TOther> handler = null) =>
            new MessageStream(this, new MessageStream<TOther>(message, handler));

        /// <inheritdoc />
        public IMessageStream AppendStream(IMessageStream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }
            if (stream.Count == 0)
            {
                return this;
            }
            return new MessageStream(this, stream);
        }

        /// <inheritdoc />
        public Task HandleMessagesWithAsync(IMessageHandler handler) =>
            handler == null ? AsyncMethod.Void : handler.HandleAsync(_message, _handler);

        #endregion
    }
}
