using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        /// <inheritdoc />
        public override string ToString() =>
            $"{_left} | {_right}";

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

        #endregion

        #region [====== IMessageStream ======]                

        /// <inheritdoc />
        public IMessageStream Append<TMessage>(TMessage message, Action<TMessage, IMicroProcessorContext> handler) =>
            new MessageStream(this, CreateStream(message, handler));

        /// <inheritdoc />
        public IMessageStream Append<TMessage>(TMessage message, Func<TMessage, IMicroProcessorContext, Task> handler) =>
            new MessageStream(this, CreateStream(message, handler));

        /// <inheritdoc />
        public IMessageStream Append<TMessage>(TMessage message, IMessageHandler<TMessage> handler = null) =>
            new MessageStream(this, CreateStream(message, handler));

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

        private static readonly ConcurrentDictionary<Type, Func<object, object, IMessageStream>> _MessageStreamConstructors = new ConcurrentDictionary<Type, Func<object, object, IMessageStream>>();

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
            CreateStream(message, MessageHandler<TMessage>.FromDelegate(handler));

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
            CreateStream(message, MessageHandler<TMessage>.FromDelegate(handler));

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
        public static IMessageStream CreateStream<TMessage>(TMessage message, IMessageHandler<TMessage> handler = null)
        {
            if (ReferenceEquals(message, null))
            {
                throw new ArgumentNullException(nameof(message));
            }
            var messageType = message.GetType();
            if (messageType == typeof(TMessage))
            {
                return new MessageStream<TMessage>(message, handler);
            }
            return CreateStreamByReflection(message, handler);
        }

        private static IMessageStream CreateStreamByReflection(object message, object handler) =>
            GetOrAddMessageStreamConstructorFor(message.GetType()).Invoke(message, handler);

        private static Func<object, object, IMessageStream> GetOrAddMessageStreamConstructorFor(Type messageType) =>
            _MessageStreamConstructors.GetOrAdd(messageType, GetMessageStreamConstructorFor);

        private static Func<object, object, IMessageStream> GetMessageStreamConstructorFor(Type messageType)
        {
            var messageStreamTypeDefinition = typeof(MessageStream<>);
            var messageStreamType = messageStreamTypeDefinition.MakeGenericType(messageType);

            var messageHandlerTypeDefinition = typeof(IMessageHandler<>);
            var messageHandlerType = messageHandlerTypeDefinition.MakeGenericType(messageType);

            var constructor = messageStreamType.GetConstructor(new [] { messageType, messageHandlerType });

            var messageParameter = Expression.Parameter(typeof(object), "message");
            var message = Expression.Convert(messageParameter, messageType);
            var handlerParameter = Expression.Parameter(typeof(object), "handler");
            var handler = Expression.Convert(handlerParameter, messageHandlerType);
            var body = Expression.New(constructor, message, handler);

            return Expression.Lambda<Func<object, object, IMessageStream>>(body, messageParameter, handlerParameter).Compile();
        }
        
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
