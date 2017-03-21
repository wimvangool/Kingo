using System;
using System.Collections.Generic;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents a message that is being processed by a <see cref="IMicroProcessor" />.
    /// </summary>
    public sealed class MessageInfo : ITypeAttributeProvider
    {        
        private MessageInfo(object message, MessageSources source)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            Message = message;
            Source = source;
            TypeAttributeProvider = new TypeAttributeProvider(message.GetType());
        }

        /// <summary>
        /// Creates and returns a new <see cref="MessageInfo" /> for a message that represents the input message of a <see cref="IQuery{T, S}" />
        /// </summary>
        /// <param name="message">A message.</param>
        /// <returns>A new <see cref="MessageInfo" /> instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        public static MessageInfo FromQuery(object message) =>
            new MessageInfo(message, MessageSources.Query);

        /// <summary>
        /// Creates and returns a new <see cref="MessageInfo" /> for a message that was passed directly to the <see cref="IMicroProcessor" />.
        /// </summary>
        /// <param name="message">A message.</param>
        /// <returns>A new <see cref="MessageInfo" /> instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        public static MessageInfo FromInputStream(object message) =>
            new MessageInfo(message, MessageSources.InputStream);

        /// <summary>
        /// Creates and returns a new <see cref="MessageInfo" /> for a message that originates from the <see cref="IMicroProcessorContext.OutputStream" />
        /// </summary>
        /// <param name="message">A message.</param>
        /// <returns>A new <see cref="MessageInfo" /> instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        public static MessageInfo FromOutputStream(object message) =>
            new MessageInfo(message, MessageSources.OutputStream);

        /// <summary>
        /// Creates and returns a new <see cref="MessageInfo" /> for a message that originates from the <see cref="IMicroProcessorContext.MetadataStream" />
        /// </summary>
        /// <param name="message">A message.</param>
        /// <returns>A new <see cref="MessageInfo" /> instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        public static MessageInfo FromMetadataStream(object message) =>
            new MessageInfo(message, MessageSources.MetadataStream);

        #region [====== Message & Source ======]

        /// <summary>
        /// Returns the associated message.
        /// </summary>
        public object Message
        {
            get;
        }

        /// <summary>
        /// Returns the source of a message.
        /// </summary>
        public MessageSources Source
        {
            get;
        }

        public override string ToString() =>
            $"{Message.GetType().FriendlyName()} ({Source})";

        #endregion

        #region [====== IsInstanceOf ======]

        /// <summary>
        /// Indicates whether or not the message is an instance of the specified type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">A type.</typeparam>
        /// <returns><c>true</c> if the message is an instance of the specified type <typeparamref name="T"/>; otherwise <c>false</c>.</returns>
        public bool IsInstanceOf<T>() =>
            IsInstanceOf(typeof(T));

        /// <summary>
        /// Indicates whether or not the message is an instance of the specified type <typeparamref name="T"/> and returns the message in its casted form
        /// if the cast succeeded.
        /// </summary>
        /// <typeparam name="T">A type.</typeparam>
        /// <param name="message">
        /// If the cast succeeds, this parameter will be assigned to the casted version of the message; otherwise it will be assigned the default
        /// value of <typeparamref name="T"/>.
        /// </param>
        /// <returns><c>true</c> if the message is an instance of the specified type <typeparamref name="T"/>; otherwise <c>false</c>.</returns>
        public bool IsInstanceOf<T>(out T message)
        {
            try
            {
                message = (T) Message;
                return true;
            }
            catch
            {
                message = default(T);
                return false;
            }
        }

        /// <summary>
        /// Indicates whether or not the message is an instance of the specified <paramref name="type"/>.
        /// </summary>
        /// <param name="type">A type.</param>
        /// <returns><c>true</c> if the message is an instance of the specified <paramref name="type"/>; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public bool IsInstanceOf(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            return type.IsInstanceOfType(Message);
        }

        #endregion

        #region [====== ITypeAttributeProvider ======]

        private TypeAttributeProvider TypeAttributeProvider
        {
            get;
        }

        /// <inheritdoc />
        public bool TryGetTypeAttributeOfType<TAttribute>(out TAttribute attribute) where TAttribute : class =>
            TypeAttributeProvider.TryGetTypeAttributeOfType(out attribute);

        /// <inheritdoc />
        public IEnumerable<TAttribute> GetTypeAttributesOfType<TAttribute>() where TAttribute : class =>
            TypeAttributeProvider.GetTypeAttributesOfType<TAttribute>();

        #endregion
    }
}
