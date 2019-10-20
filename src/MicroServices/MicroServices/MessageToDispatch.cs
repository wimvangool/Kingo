using System;
using System.Text;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents a command or event that is scheduled to be sent or published on the service-bus.
    /// </summary>
    public sealed class MessageToDispatch : IMessageToDispatch, IMessageToProcess
    {
        private readonly IMessageEnvelope _message;
        private readonly MessageKind _kind;
        private readonly DateTimeOffset? _deliveryTimeUtc;

        internal MessageToDispatch(IMessageEnvelope message, MessageKind kind, DateTimeOffset? deliveryTimeUtc)
        {
            _message = message;
            _kind = kind.Validate();
            _deliveryTimeUtc = deliveryTimeUtc?.ToUniversalTime();
        }

        private MessageToDispatch(MessageToDispatch message, IMessageEnvelope correlatedMessage)
        {
            _message = new MessageEnvelope(message.Content, message.MessageId, correlatedMessage.MessageId);
            _kind = message._kind;
            _deliveryTimeUtc = message._deliveryTimeUtc;
        }

        /// <inheritdoc />
        public string MessageId =>
            _message.MessageId;

        /// <inheritdoc />
        public string CorrelationId =>
            _message.CorrelationId;

        /// <inheritdoc />
        public object Content =>
            _message.Content;

        /// <summary>
        /// Indicates which kind of message is to be dispatched.
        /// </summary>
        public MessageKind Kind =>
            _kind;

        /// <summary>
        /// If specified, indicates at what (UTC) time the message should be sent or published on the service-bus.
        /// </summary>
        public DateTimeOffset? DeliveryTimeUtc =>
            _deliveryTimeUtc;

        /// <inheritdoc />
        public override string ToString() =>
            ToString(this);

        internal static string ToString<TMessage>(TMessage message) where TMessage : IMessageToProcess, IMessageToDispatch
        {
            var messageBuilder = new StringBuilder(message.Content.GetType().FriendlyName());

            messageBuilder.Append(" (").Append($"{nameof(Kind)} = {message.Kind}");

            if (message.DeliveryTimeUtc.HasValue)
            {
                messageBuilder.Append($"{nameof(DeliveryTimeUtc)} = {message.DeliveryTimeUtc}");
            }
            return messageBuilder.Append(")").ToString();
        }

        /// <summary>
        /// Creates and returns a copy of this message that is correlated with the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message this message is correlated message.</param>
        /// <returns>A copy of this message that is correlated with the specified <paramref name="message"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        public MessageToDispatch CorrelateWith(IMessageEnvelope message) =>
            new MessageToDispatch(this, message ?? throw new ArgumentNullException(nameof(message)));

        /// <summary>
        /// Checks if the message of this message is of type <typeparamref name="TMessage"/> and if so,
        /// convert this message into a message of that type.
        /// </summary>
        /// <typeparam name="TMessage">A message type.</typeparam>
        /// <param name="messageToDispatch">
        /// If <see cref="Content"/> is of type <typeparamref name="TMessage"/>, this argument will be set to
        /// an instance of <see cref="MessageToDispatch{TMessage}"/>; otherwise it will be <c>null</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if <see cref="Content"/> is of type <typeparamref name="TMessage"/>, otherwise <c>false</c>.
        /// </returns>
        public bool IsOfType<TMessage>(out MessageToDispatch<TMessage> messageToDispatch)
        {
            if (MessageEnvelopeExtensions.IsOfType<TMessage>(this, out var message))
            {
                messageToDispatch = new MessageToDispatch<TMessage>(message, Kind, DeliveryTimeUtc);
                return true;
            }
            messageToDispatch = null;
            return false;
        }

        /// <summary>
        /// Converts this message to dispatch to a (typed) message to process.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message message.</typeparam>
        /// <returns>A new <see cref="MessageToProcess{TMessage}"/>.</returns>
        /// <exception cref="InvalidCastException">
        /// <see cref="Content"/> is not an instance of type <typeparamref name="TMessage"/>.
        /// </exception>
        public MessageToProcess<TMessage> ToProcess<TMessage>() =>
            new MessageEnvelope<TMessage>((TMessage) Content, MessageId, CorrelationId).ToProcess(Kind);
    }
}
