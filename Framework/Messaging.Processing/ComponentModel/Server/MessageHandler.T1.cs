using System.Threading.Tasks;

namespace System.ComponentModel.Server
{
    /// <summary>
    /// Serves as a base class for all <see cref="IMessageHandler{TMessage}">MessageHandlers</see>.
    /// </summary>
    /// <typeparam name="TMessage">Type of message to handle.</typeparam>
    public abstract class MessageHandler<TMessage> : IMessageHandler<TMessage> where TMessage : class
    {
        /// <inheritdoc />               
        public abstract Task HandleAsync(TMessage message);

        /// <summary>
        /// Publishes the specified <paramref name="message"/> as part of the current Unit of Work.
        /// </summary>
        /// <typeparam name="T">Type of the message to publish.</typeparam>
        /// <param name="message">The message to publish.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The method is not being called inside a <see cref="UnitOfWorkScope" />.
        /// </exception>
        public static void Publish<T>(T message) where T : class, IMessage<T>
        {
            MessageProcessor.Publish(message);
        }
    }
}
