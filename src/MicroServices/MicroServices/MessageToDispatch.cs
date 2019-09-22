using System;
using System.Text;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents a command or event that is to be sent or published on a service-bus.
    /// </summary>
    public sealed class MessageToDispatch : IMessageToDispatch, IMessageToProcess
    {
        private MessageToDispatch(object content, MessageKind kind, DateTimeOffset? deliveryTime = null)
        {
            Content = content;
            Kind = kind;
            DeliveryTimeUtc = deliveryTime?.ToUniversalTime();
        }

        /// <inheritdoc />
        public object Content
        {
            get;
        }

        /// <summary>
        /// Indicates which kind of message is to be dispatched.
        /// </summary>
        public MessageKind Kind
        {
            get;
        }

        /// <summary>
        /// If specified, indicates at what (UTC) time the message should be sent or published on the service-bus.
        /// </summary>
        public DateTimeOffset? DeliveryTimeUtc
        {
            get;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            var message = new StringBuilder(Content.GetType().FriendlyName());

            message.Append(" (").Append($"{nameof(Kind)} = {Kind}");

            if (DeliveryTimeUtc.HasValue)
            {
                message.Append($"{nameof(DeliveryTimeUtc)} = {DeliveryTimeUtc}");
            }
            return message.Append(")").ToString();
        }

        /// <summary>
        /// Creates and returns a command that is ready to be dispatched.
        /// </summary>
        /// <param name="command">The command to dispatch.</param>
        /// <param name="deliveryTime">
        /// If specified, indicates at what (UTC) time the message should be sent or published on the service-bus.
        /// </param>
        /// <returns>The message to dispatch.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="command"/> is <c>null</c>.
        /// </exception>
        public static MessageToDispatch CreateCommand(object command, DateTimeOffset? deliveryTime = null) =>
            new MessageToDispatch(command ?? throw new ArgumentNullException(nameof(command)), MessageKind.Command, deliveryTime);

        /// <summary>
        /// Creates and returns an event that is ready to be dispatched.
        /// </summary>
        /// <param name="event">The event to dispatch.</param>
        /// <param name="deliveryTime">
        /// If specified, indicates at what (UTC) time the message should be sent or published on the service-bus.
        /// </param>
        /// <returns>The message to dispatch.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="event"/> is <c>null</c>.
        /// </exception>
        public static MessageToDispatch CreateEvent(object @event, DateTimeOffset? deliveryTime = null) =>
            new MessageToDispatch(@event ?? throw new ArgumentNullException(nameof(@event)), MessageKind.Event, deliveryTime);
    }
}
