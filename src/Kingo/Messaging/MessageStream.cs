using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kingo.Messaging.Validation;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents a stream of <see cref="IMessage">messages</see>.
    /// </summary>
    public sealed class MessageStream : ReadOnlyList<object>, IMessageStream
    {
        /// <summary>
        /// Represents an empty stream.
        /// </summary>
        public static readonly IMessageStream Empty = new EmptyMessageStream();

        private readonly IMessageStream _left;
        private readonly IMessageStream _right;
        private int? _length;

        internal MessageStream(IMessageStream left, IMessageStream right)
        {            
            _left = left;
            _right = right;
        }

        #region [====== IReadOnlyList<object>  ======]

        /// <inheritdoc />
        public override int Count
        {
            get
            {
                if (!_length.HasValue)
                {
                    _length = _left.Count + _right.Count;
                }
                return _length.Value;
            }
        }        

        /// <inheritdoc />
        public override IEnumerator<object> GetEnumerator() =>
            _left.Concat(_right).GetEnumerator();

        /// <inheritdoc />
        public override string ToString() =>
            $"{Count} message(s)";

        #endregion

        #region [====== IMessageStream ======]                

        /// <inheritdoc />
        public IMessageStream Append<TMessage>(TMessage message, Action<TMessage, IMicroProcessorContext> handler) =>
            new MessageStream(this, new MessageStream<TMessage>(message, handler));

        /// <inheritdoc />
        public IMessageStream Append<TMessage>(TMessage message, Func<TMessage, IMicroProcessorContext, Task> handler) =>
            new MessageStream(this, new MessageStream<TMessage>(message, handler));

        /// <inheritdoc />
        public IMessageStream Append<TMessage>(TMessage message, IMessageHandler<TMessage> handler = null) =>
            new MessageStream(this, new MessageStream<TMessage>(message, handler));

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
        public async Task HandleMessagesWithAsync(IMessageHandler handler)
        {
            await _left.HandleMessagesWithAsync(handler);
            await _right.HandleMessagesWithAsync(handler);           
        }

        #endregion

        /// <summary>
        /// Creates and returns a new stream while adding the specified <paramref name="message"/> to this stream
        /// and associating it with the specified <paramref name="handler"/>.
        /// </summary>        
        /// <param name="message">Message to append.</param>
        /// <param name="handler">Optional handler to associate with the message.</param>
        /// <returns>A new stream containing the appended message.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        public static IMessageStream CreateStream<TMessage>(TMessage message, Action<TMessage, IMicroProcessorContext> handler) =>
            Empty.Append(message, handler);

        /// <summary>
        /// Creates and returns a new stream while adding the specified <paramref name="message"/> to this stream
        /// and associating it with the specified <paramref name="handler"/>.
        /// </summary>        
        /// <param name="message">Message to append.</param>
        /// <param name="handler">Optional handler to associate with the message.</param>
        /// <returns>A new stream containing the appended message.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        public static IMessageStream CreateStream<TMessage>(TMessage message, Func<TMessage, IMicroProcessorContext, Task> handler) =>
            Empty.Append(message, handler);

        /// <summary>
        /// Creates and returns a new stream while adding the specified <paramref name="message"/> to this stream
        /// and associating it with the specified <paramref name="handler"/>.
        /// </summary>        
        /// <param name="message">Message to append.</param>
        /// <param name="handler">Optional handler to associate with the message.</param>
        /// <returns>A new stream containing the appended message.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        public static IMessageStream CreateStream<TMessage>(TMessage message, IMessageHandler<TMessage> handler = null) =>
            Empty.Append(message, handler);
        
        internal static IMessageStream Concat(IEnumerable<IMessageStream> streams)
        {
            if (streams == null)
            {
                throw new ArgumentNullException(nameof(streams));
            }
            IMessageStream stream = new EmptyMessageStream();

            foreach (var streamElement in streams.WhereNotNull())
            {
                stream = stream.AppendStream(streamElement);
            }
            return stream;
        }
    }
}
