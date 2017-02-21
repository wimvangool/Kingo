using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Kingo.Resources;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents a stream of <see cref="IMessage">messages</see>.
    /// </summary>
    public sealed class MessageStream : IReadOnlyList<IMessage>
    {
        /// <summary>
        /// Represents an empty stream.
        /// </summary>
        public static readonly MessageStream Empty = new MessageStream();

        private readonly MessageStreamElement _element;

        private MessageStream()
            : this(new MessageStreamNullElement()) { }

        private MessageStream(MessageStreamElement element)
        {
            _element = element;
        }

        /// <summary>
        /// Lets the specified <paramref name="handler"/> iterate all messages of this stream.
        /// </summary>
        /// <param name="handler">A visitor that will visit each message.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="handler"/> is <c>null</c>.
        /// </exception>
        public void Accept(IMessageHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }
            _element.Accept(handler);
        }

        #region [====== IReadOnlyList  ======]

        /// <inheritdoc />
        public int Count => _element.Count();

        /// <inheritdoc />
        public IMessage this[int index]
        {
            get
            {
                IMessage message;

                if (_element.TryGetItem(index, out message))
                {
                    return message;
                }
                throw NewIndexOutOfRangeException(index, Count);
            }
        }

        private static IndexOutOfRangeException NewIndexOutOfRangeException(int index, int count)
        {
            var messageFormat = ExceptionMessages.MessageStream_IndexOutOfRange;
            var message = string.Format(messageFormat, index, count);
            return new IndexOutOfRangeException(message);
        }

        /// <inheritdoc />
        public IEnumerator<IMessage> GetEnumerator() => _element.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _element.GetEnumerator();

        #endregion

        #region [====== AppendStream ======]

        /// <summary>
        /// Appends the specified <paramref name="messages" /> to the current stream and returns the resulting stream.
        /// </summary>
        /// <param name="messages">The stream to append.</param>
        /// <returns>A new stream representing both the current and the specified <paramref name="messages"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messages"/> is <c>null</c>.
        /// </exception>
        public MessageStream AppendStream(IEnumerable<IMessage> messages)
        {
            return AppendStream(FromMessages(messages));
        }

        /// <summary>
        /// Appends the specified <paramref name="stream" /> to the current stream and returns the resulting stream.
        /// </summary>
        /// <param name="stream">The stream to append.</param>
        /// <returns>A new stream representing both the current and the specified <paramref name="stream"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream"/> is <c>null</c>.
        /// </exception>
        public MessageStream AppendStream(MessageStream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }
            return new MessageStream(_element.Append(stream._element));
        }

        /// <summary>
        /// Combines two streams into a single stream.
        /// </summary>
        /// <param name="left">The first stream.</param>
        /// <param name="right">The second stream.</param>
        /// <returns>
        /// A new stream representing the combination of <paramref name="left"/> and <paramref name="right"/>.
        /// If both are <c>null</c>, the resulting stream is also <c>null</c>.
        /// </returns>
        public static MessageStream operator +(MessageStream left, MessageStream right)
        {
            if (ReferenceEquals(left, null))
            {
                return right;
            }
            if (ReferenceEquals(right, null))
            {
                return left;
            }
            return left.AppendStream(right);
        }

        #endregion

        #region [====== Append ======]

        private static readonly Lazy<MethodInfo> _AppendMethodDefinition = new Lazy<MethodInfo>(FindAppendMethodDefinition, true);
        private static readonly ConcurrentDictionary<Type, Func<MessageStream, IMessage, MessageStream>> _AppendMethods = new ConcurrentDictionary<Type, Func<MessageStream, IMessage, MessageStream>>();

        private static MethodInfo FindAppendMethodDefinition()
        {
            var methods =
                from method in typeof(MessageStream).GetMethods(BindingFlags.Public | BindingFlags.Instance)
                where method.Name == nameof(Append) && method.IsGenericMethodDefinition
                select method;

            return methods.Single();
        }        

        /// <summary>
        /// Appends the specified <paramref name="message"/> to the end of the stream and returns the resulting stream.
        /// </summary>
        /// <param name="message">The message to append.</param>
        /// <returns>A new stream containing the appended message at the end of the stream.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        public MessageStream Append(IMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));              
            }
            return _AppendMethods.GetOrAdd(message.GetType(), CreateAppendMethod).Invoke(this, message);                           
        }

        private static Func<MessageStream, IMessage, MessageStream> CreateAppendMethod(Type messageType)
        {
            var messageStreamParameter = Expression.Parameter(typeof(MessageStream), "stream");
            var messageParameter = Expression.Parameter(typeof(IMessage), "message");
            var message = Expression.Convert(messageParameter, messageType);

            var appendMethod = _AppendMethodDefinition.Value.MakeGenericMethod(messageType);
            var appendMethodCall = Expression.Call(messageStreamParameter, appendMethod, message);
            var expression = Expression.Lambda<Func<MessageStream, IMessage, MessageStream>>(appendMethodCall, messageStreamParameter, messageParameter);

            return expression.Compile();
        }

        /// <summary>
        /// Appends the specified <paramref name="message"/> to the end of the stream and returns the resulting stream.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message to append.</typeparam>
        /// <param name="message">The message to append.</param>
        /// <returns>A new stream containing the appended message at the end of the stream.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        public MessageStream Append<TMessage>(TMessage message) where TMessage : class, IMessage
        {
            return new MessageStream(_element.Append(message));
        }

        /// <summary>
        /// Combines a stream and a message into a new stream.
        /// </summary>
        /// <param name="left">A stream.</param>
        /// <param name="right">A message.</param>
        /// <returns>A new stream.</returns>
        public static MessageStream operator +(MessageStream left, Message right)
        {
            if (ReferenceEquals(left, null))
            {
                return right;
            }
            if (ReferenceEquals(right, null))
            {
                return left;
            }
            return left.Append(right as IMessage);
        }

        /// <summary>
        /// Combines a message and a stream into a new stream.
        /// </summary>
        /// <param name="left">A message.</param>
        /// <param name="right">A stream.</param>
        /// <returns>A new stream.</returns>
        public static MessageStream operator +(Message left, MessageStream right)
        {
            if (ReferenceEquals(left, null))
            {
                return right;
            }
            if (ReferenceEquals(right, null))
            {
                return left;
            }
            return FromMessage(left as IMessage).AppendStream(right);
        }

        #endregion

        #region [====== FromMessage(s)======]

        /// <summary>
        /// Creates a new stream containing all specified <paramref name="messages"/>
        /// </summary>
        /// <param name="messages">A collection of messages.</param>
        /// <returns>A new stream containing all specified <paramref name="messages."/></returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messages"/> is <c>null</c>.
        /// </exception>
        public static MessageStream FromMessages(IEnumerable<IMessage> messages)
        {
            var stream = Empty;

            foreach (var message in messages.WhereNotNull())
            {
                stream = stream.Append(message);
            }
            return stream;
        }

        /// <summary>
        /// Creates a new stream containing the specified <paramref name="message"/>.
        /// </summary>        
        /// <param name="message">A message.</param>
        /// <returns>A new stream containing the specified <paramref name="message"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        public static MessageStream FromMessage(IMessage message)
        {
            return Empty.Append(message);
        }

        /// <summary>
        /// Creates a new stream containing the specified <paramref name="message"/>.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message.</typeparam>
        /// <param name="message">A message.</param>
        /// <returns>A new stream containing the specified <paramref name="message"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        public static MessageStream FromMessage<TMessage>(TMessage message) where TMessage : class, IMessage
        {
            return new MessageStream(new MessageStreamMessageElement<TMessage>(message));
        }

        /// <summary>
        /// Implicitly converts the specified <paramref name="value"/> to a stream.
        /// </summary>
        /// <param name="value">The message to convert.</param>
        public static implicit operator MessageStream(Message value)
        {
            return ReferenceEquals(value, null) ? null : FromMessage(value as IMessage);
        }

        #endregion
    }
}
