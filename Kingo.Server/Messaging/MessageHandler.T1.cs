using System;
using System.Threading.Tasks;

namespace Kingo.Messaging
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
        /// Publishes the specified <paramref name="message"/> as part of the current Unit of Work,
        /// if a <see cref="UnitOfWorkContext" /> is active.
        /// </summary>
        /// <typeparam name="TEvent">Type of the message to publish.</typeparam>
        /// <param name="message">The message to publish.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>        
        protected virtual void Publish<TEvent>(TEvent message) where TEvent : class, IMessage<TEvent>
        {
            var context = UnitOfWorkContext.Current;
            if (context != null)
            {
                context.Publish(message);
            }
        }
    }
}
