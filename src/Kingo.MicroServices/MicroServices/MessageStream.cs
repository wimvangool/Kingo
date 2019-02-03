using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Kingo.Collections.Generic;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents a stream of messages.
    /// </summary>
    public abstract class MessageStream : ReadOnlyList<object>
    {
        #region [====== Empty ======]

        private sealed class EmptyStream : MessageStream
        {
            public override int Count =>
                0;

            public override IEnumerator<object> GetEnumerator() =>
                Enumerable.Empty<object>().GetEnumerator();                        

            public override MessageStream Concat(MessageStream stream) =>
                stream ?? this;

            public override Task<MessageStream> HandleWithAsync(IMessageHandler handler)
            {
                if (handler == null)
                {
                    throw new ArgumentNullException(nameof(handler));
                }
                return Task.FromResult(Empty);
            }
        }

        /// <summary>
        /// Represents an empty stream.
        /// </summary>
        public static readonly MessageStream Empty = new EmptyStream();

        #endregion

        #region [====== Append  & Concat ======]  

        /// <summary>
        /// Appends all specified <paramref name="messages"/> to the end of this stream and returns the resulting stream.
        /// If messages is <c>null</c> or empty, nothing is appended.
        /// </summary>
        /// <param name="messages">The messages to append.</param>
        /// <returns>A new stream containing all specified <paramref name="messages"/>.</returns>        
        public MessageStream Append(params object[] messages) =>
            Append(messages as IEnumerable<object>);

        /// <summary>
        /// Appends all specified <paramref name="messages"/> to the end of this stream and returns the resulting stream.
        /// If messages is <c>null</c> or empty, nothing is appended.
        /// </summary>
        /// <param name="messages">The messages to append.</param>
        /// <returns>A new stream containing all specified <paramref name="messages"/>.</returns>        
        public MessageStream Append(IEnumerable<object> messages)
        {            
            var stream = this;

            foreach (var message in messages ?? Enumerable.Empty<object>())
            {
                stream = stream.Append(message);
            }
            return stream;
        }

        /// <summary>
        /// Appends the specified <paramref name="message"/> to the end of this stream and returns the resulting stream.
        /// If <paramref name="message"/> is <c>null</c>, nothing is appended.
        /// </summary>        
        /// <param name="message">Message to append.</param>
        /// <returns>A new stream where <paramref name="message"/> has been appended.</returns>        
        public MessageStream Append(object message) =>
            message == null ? this : Concat(CreateStream(message));

        /// <summary>
        /// Concatenates this stream with the specified <paramref name="stream"/> and returns the resulting stream.
        /// If <paramref name="stream"/> is <c>null</c> or empty, nothing is concatenated to this stream.
        /// </summary>
        /// <param name="stream">Stream to append.</param>
        /// <returns>A new stream where the specified <paramref name="stream"/> has been added to.</returns>        
        public virtual MessageStream Concat(MessageStream stream)
        {
            if (stream == null || stream.Count == 0)
            {
                return this;
            }           
            return new MessageStreamPair(this, stream);
        }

        /// <summary>
        /// Concatenates all specified <paramref name="streams"/> and returns the resulting stream.
        /// </summary>
        /// <param name="streams">The streams to concatenate.</param>
        /// <returns>A new stream containing all messages of the specified <paramref name="streams"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="streams"/> is <c>null</c>.
        /// </exception>
        public static MessageStream Concat(params MessageStream[] streams) =>
            Concat(streams as IEnumerable<MessageStream>);

        /// <summary>
        /// Concatenates all specified <paramref name="streams"/> and returns the resulting stream.
        /// </summary>
        /// <param name="streams">The streams to concatenate.</param>
        /// <returns>A new stream containing all messages of the specified <paramref name="streams"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="streams"/> is <c>null</c>.
        /// </exception>
        public static MessageStream Concat(IEnumerable<MessageStream> streams)
        {
            if (streams == null)
            {
                throw new ArgumentNullException(nameof(streams));
            }
            var stream = Empty;

            foreach (var streamToAppend in streams)
            {
                stream = stream.Concat(streamToAppend);
            }
            return stream;
        }

        /// <summary>
        /// Concatenates the specified <paramref name="stream"/> and <paramref name="message"/> into a new stream.
        /// </summary>
        /// <param name="stream">A stream.</param>
        /// <param name="message">A message.</param>
        /// <returns>A new stream.</returns>
        public static MessageStream operator +(MessageStream stream, object message) =>
            EnsureNotNull(stream).Append(message);

        /// <summary>
        /// Concatenates the specified <paramref name="message"/> and <paramref name="stream"/> into a new stream.
        /// </summary>        
        /// <param name="message">A message.</param>
        /// <param name="stream">A stream.</param>
        /// <returns>A new stream.</returns>
        public static MessageStream operator +(object message, MessageStream stream) =>
            EnsureNotNull(message).Concat(stream);

        /// <summary>
        /// Concatenates the specified <paramref name="stream"/> and <paramref name="messages"/> into a new stream.
        /// </summary>
        /// <param name="stream">A stream.</param>
        /// <param name="messages">A collection of messages.</param>
        /// <returns>A new stream.</returns>
        public static MessageStream operator +(MessageStream stream, IEnumerable<object> messages) =>
            EnsureNotNull(stream).Append(messages);

        /// <summary>
        /// Concatenates the specified <paramref name="messages"/> and <paramref name="stream"/> into a new stream.
        /// </summary>        
        /// <param name="messages">A collection of messages.</param>
        /// <param name="stream">A stream.</param>
        /// <returns>A new stream.</returns>
        public static MessageStream operator +(IEnumerable<object> messages, MessageStream stream) =>
            EnsureNotNull(messages).Concat(stream);

        /// <summary>
        /// Concatenates both streams.
        /// </summary>
        /// <param name="left">Left stream.</param>
        /// <param name="right">Right stream.</param>
        /// <returns>A new stream.</returns>
        public static MessageStream operator +(MessageStream left, MessageStream right) =>
            EnsureNotNull(left).Concat(right);

        private static MessageStream EnsureNotNull(object message) =>
            message == null ? Empty : CreateStream(message);

        private static MessageStream EnsureNotNull(IEnumerable<object> messages) =>
            messages == null ? Empty : CreateStream(messages);

        private static MessageStream EnsureNotNull(MessageStream stream) =>
            stream ?? Empty;

        #endregion

        #region [====== HandleWith ======]

        /// <summary>
        /// Handles all messages of this stream with the specified <paramref name="handler"/>
        /// and returns the resulting stream of events.
        /// </summary>
        /// <param name="handler">Handler to invoke for each message in this stream.</param>
        /// <returns>
        /// A concatenated stream of events that is the result of processing each message with
        /// the specified <paramref name="handler"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="handler"/> is <c>null</c>.
        /// </exception>
        public abstract Task<MessageStream> HandleWithAsync(IMessageHandler handler);

        #endregion

        #region [====== ToString ======]

        /// <inheritdoc />
        public override string ToString() =>
            '{' + string.Join(" | ", this.Select(message => message.GetType().FriendlyName())) + '}';

        #endregion

        #region [====== CreateStream ======]

        private static readonly ConcurrentDictionary<Type, Func<object, MessageStream>> _MessageStreamConstructors = new ConcurrentDictionary<Type, Func<object, MessageStream>>();

        /// <summary>
        /// Creates and returns a new stream containing all specified <paramref name="messages"/>.
        /// </summary>
        /// <param name="messages">Messages to add to the stream.</param>
        /// <returns>A new stream containing all the messages.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messages"/> is <c>null</c>.
        /// </exception>
        public static MessageStream CreateStream(params object[] messages) =>
            Empty.Append(messages);

        /// <summary>
        /// Creates and returns a new stream containing all specified <paramref name="messages"/>.
        /// </summary>
        /// <param name="messages">Messages to add to the stream.</param>
        /// <returns>A new stream containing all the messages.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messages"/> is <c>null</c>.
        /// </exception>
        public static MessageStream CreateStream(IEnumerable<object> messages) =>
            Empty.Append(messages);

        /// <summary>
        /// Creates and returns a new stream containing the specified <paramref name="message"/>.
        /// </summary>        
        /// <param name="message">Message to append.</param>        
        /// <returns>A new stream containing the appended message.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        public static MessageStream CreateStream(object message) =>
            GetOrAddMessageStreamConstructorFor((message ?? throw new ArgumentNullException(nameof(message))).GetType()).Invoke(message);

        private static Func<object, MessageStream> GetOrAddMessageStreamConstructorFor(Type messageType) =>
            _MessageStreamConstructors.GetOrAdd(messageType, GetMessageStreamConstructorFor);

        private static Func<object, MessageStream> GetMessageStreamConstructorFor(Type messageType)
        {
            var messageStreamTypeDefinition = typeof(MessageStream<>);
            var messageStreamType = messageStreamTypeDefinition.MakeGenericType(messageType);            

            var constructor = messageStreamType.GetConstructor(new [] { messageType });
            var messageParameter = Expression.Parameter(typeof(object), "message");
            var message = Expression.Convert(messageParameter, messageType);           
            var body = Expression.New(constructor, message);

            return Expression.Lambda<Func<object, MessageStream>>(body, messageParameter).Compile();
        }

        #endregion
    }
}
