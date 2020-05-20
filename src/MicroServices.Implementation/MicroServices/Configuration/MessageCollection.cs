using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Kingo.Reflection;
using Microsoft.Extensions.DependencyInjection;
using static Kingo.Ensure;

namespace Kingo.MicroServices.Configuration
{
    /// <summary>
    /// Represents a collection of messages and associated configuration that can be used to
    /// configure a <see cref="IMicroProcessor"/> on how to process different types of messages.
    /// </summary>
    public sealed class MessageCollection : IMicroProcessorComponentCollection
    {
        private readonly Dictionary<MessageDirection, MessageValidationOptions> _messageValidationOptions;
        private readonly Dictionary<Type, MessageAttribute> _attributes;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageCollection" /> class.
        /// </summary>
        public MessageCollection()
        {
            _attributes = new Dictionary<Type, MessageAttribute>();
            _messageValidationOptions = new Dictionary<MessageDirection, MessageValidationOptions>();
            _messageKindResolver = new DefaultMessageKindResolver();
            _messageIdGenerator = new DefaultMessageIdGenerator();
        }

        /// <inheritdoc />
        public override string ToString() =>
            $"[{nameof(Type)} --> {nameof(MessageAttribute)}] ({_attributes.Count} specific mapping(s) added)";

        #region [====== IEnumerable<MicroProcessorComponent> ======]

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        /// <inheritdoc />
        public IEnumerator<MicroProcessorComponent> GetEnumerator() =>
            Enumerable.Empty<MicroProcessorComponent>().GetEnumerator();

        #endregion

        #region [====== AddSpecificComponentsTo ======]

        IServiceCollection IMicroProcessorComponentCollection.AddSpecificComponentsTo(IServiceCollection services) =>
            services.AddSingleton(BuildMessageFactory());

        internal MessageFactory BuildMessageFactory()
        {
            var messageKindResolver = new MessageKindResolver(_attributes, _messageKindResolver);
            var messageIdGenerator = new MessageIdGenerator(_attributes, _messageIdGenerator);
            return new MessageFactory(_messageValidationOptions, messageKindResolver, messageIdGenerator);
        }

        #endregion

        #region [====== MessageValidationOptions ======]

        /// <summary>
        /// Configures the processor to use the specified <paramref name="validationOptions"/> for messages that
        /// flow in the specified <paramref name="direction" />.
        /// </summary>
        /// <param name="direction">The direction of the messages.</param>
        /// <param name="validationOptions">The options to apply.</param>
        public void Validate(MessageDirection direction, MessageValidationOptions validationOptions) =>
            _messageValidationOptions[direction] = validationOptions;

        #endregion

        #region [====== MessageKindResolver ======]        

        private IMessageKindResolver _messageKindResolver;

        /// <summary>
        /// Gets or sets the <see cref="IMessageKindResolver" /> that will be used by the
        /// <see cref="MessageFactory" /> to resolve the <see cref="MessageKind" /> of a message
        /// for which the message-kind has not been explicitly specified.
        /// </summary>
        public IMessageKindResolver MessageKindResolver
        {
            get => _messageKindResolver;
            set => _messageKindResolver = IsNotNull(value);
        }

        #endregion

        #region [====== MessageIdGenerator ======]

        private IMessageIdGenerator _messageIdGenerator;

        /// <summary>
        /// Gets or sets the <see cref="IMessageIdGenerator" /> that will be used by the
        /// <see cref="MessageFactory" /> to generate a new message-id for a message
        /// for which no explicit <see cref="MessageAttribute.MessageIdFormat"/> has been
        /// specified.
        /// </summary>
        public IMessageIdGenerator MessageIdGenerator
        {
            get => _messageIdGenerator;
            set => _messageIdGenerator = IsNotNull(value);
        }

        #endregion

        #region [====== MessageAttributes ======]

        /// <summary>
        /// Maps the specified <typeparamref name="TMessage"/> to the specified <paramref name="messageKind"/> and <paramref name="messageIdFormat"/>.
        /// </summary>
        /// <typeparam name="TMessage">A message type.</typeparam>
        /// <param name="messageKind">Indicates what <see cref="MessageKind">kind</see> of message the specified <typeparamref name="TMessage"/> is.</param>
        /// <param name="messageIdFormat">
        /// If specified, represents a format string that will be used to generate the <see cref="IMessage.Id"/>
        /// of the message at run-time.
        /// </param>
        /// <param name="messageIdProperties">
        /// A collection of property-names that will be used to insert the required values into the
        /// specified <paramref name="messageIdFormat"/> when formatting the <see cref="IMessage.Id" />.
        /// </param>
        /// <exception cref="ArgumentException">
        /// A mapping for the specified <typeparamref name="TMessage"/> already exists.
        /// </exception>
        public void Add<TMessage>(MessageKind messageKind, string messageIdFormat = null, params string[] messageIdProperties) =>
            Add(typeof(TMessage), messageKind, messageIdFormat, messageIdProperties);

        /// <summary>
        /// Maps the specified <paramref name="messageType"/> to the specified <paramref name="messageKind"/> and <paramref name="messageIdFormat"/>.
        /// </summary>
        /// <param name="messageType">A message type.</param>
        /// <param name="messageKind">Indicates what <see cref="MessageKind">kind</see> of message the specified <paramref name="messageType"/> is.</param>
        /// <param name="messageIdFormat">
        /// If specified, represents a format string that will be used to generate the <see cref="IMessage.Id"/>
        /// of the message at run-time.
        /// </param>
        /// <param name="messageIdProperties">
        /// A collection of property-names that will be used to insert the required values into the
        /// specified <paramref name="messageIdFormat"/> when formatting the <see cref="IMessage.Id" />.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messageType"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// A mapping for the specified <paramref name="messageType"/> already exists.
        /// </exception>
        public void Add(Type messageType, MessageKind messageKind, string messageIdFormat = null, params string[] messageIdProperties) =>
            Add(messageType, new MessageAttribute(messageKind, messageIdFormat, messageIdProperties));

        /// <summary>
        /// Maps the specified <typeparamref name="TMessage"/> to the specified <paramref name="messageAttribute"/>.
        /// </summary>
        /// <typeparam name="TMessage">A message type.</typeparam>
        /// <param name="messageAttribute">A <see cref="MessageAttribute" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messageAttribute"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// A mapping for the specified <typeparamref name="TMessage"/> already exists.
        /// </exception>
        public void Add<TMessage>(MessageAttribute messageAttribute) =>
            Add(typeof(TMessage), messageAttribute);

        /// <summary>
        /// Maps the specified <paramref name="messageType"/> to the specified <paramref name="messageAttribute"/>.
        /// </summary>
        /// <param name="messageType">A message type.</param>
        /// <param name="messageAttribute">A <see cref="MessageAttribute" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messageType"/> or <paramref name="messageAttribute"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// A mapping for the specified <paramref name="messageType"/> already exists.
        /// </exception>
        public void Add(Type messageType, MessageAttribute messageAttribute)
        {
            try
            {
                _attributes.Add(IsNotNull(messageType, nameof(messageType)), IsNotNull(messageAttribute, nameof(messageAttribute)));
            }
            catch (ArgumentNullException)
            {
                throw;
            }
            catch (ArgumentException exception)
            {
                throw NewMappingForMessageTypeAlreadyExistsException(messageType, exception);
            }
        }

        private static Exception NewMappingForMessageTypeAlreadyExistsException(Type messageType, Exception exception)
        {
            var messageFormat = ExceptionMessages.MessageFactoryBuilder_MappingForMessageTypeAlreadyExists;
            var message = string.Format(messageFormat, messageType.FriendlyName());
            return new ArgumentException(message, nameof(messageType), exception);
        }

        #endregion
    }
}
