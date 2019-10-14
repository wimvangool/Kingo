using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Contains extension methods for object of type <see cref="IMessage" />.
    /// </summary>
    public static class MessageExtensions
    {
        /// <summary>
        /// Converts the specified <paramref name="message"/> to a message of a specific <paramref name="kind"/> that can be dispatched.
        /// </summary>
        /// <param name="message">The message to convert.</param>
        /// <param name="kind">Indicates what kind of message the message represents.</param>
        /// <param name="deliveryTime">
        /// If specified, indicates at what time the message should be dispatched on the service-bus.
        /// </param>
        /// <returns>The message to dispatch.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="kind"/> is not a recognized <see cref="MessageKind"/>.
        /// </exception>
        public static MessageToDispatch ToDispatch(this IMessage message, MessageKind kind, DateTimeOffset? deliveryTime = null) =>
            new MessageToDispatch(NotNull(message), kind, deliveryTime);

        /// <summary>
        /// Checks if the content of this message is of type <typeparamref name="TMessage"/> and if so,
        /// convert this message into a message of that type.
        /// </summary>
        /// <typeparam name="TMessage">A message type.</typeparam>
        /// <param name="message">The message to convert.</param>
        /// <param name="typedMessage">
        /// If <see cref="IMessage.Content"/> is of type <typeparamref name="TMessage"/>, this argument will be set to
        /// an instance of <see cref="Message{TMessage}"/>; otherwise it will be <c>null</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if <see cref="IMessage.Content"/> is of type <typeparamref name="TMessage"/>, otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        public static bool IsOfType<TMessage>(this IMessage message, out Message<TMessage> typedMessage)
        {
            if (NotNull(message).Content is TMessage content)
            {
                typedMessage = new Message<TMessage>(content, message.MessageId, message.CorrelationId);
                return true;
            }
            typedMessage = null;
            return false;
        }

        private static IMessage NotNull(IMessage message) =>
            message ?? throw new ArgumentNullException(nameof(message));
    }
}
