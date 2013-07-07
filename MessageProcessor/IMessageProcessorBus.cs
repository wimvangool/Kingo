using System;

namespace YellowFlare.MessageProcessing
{
    /// <summary>
    /// When implemented by a class, represents a <see cref="MessageProcessorBus" /> to which event-handlers can subscribe.
    /// </summary>
    public interface IMessageProcessorBus : IDomainEventBus
    {
        /// <summary>
        /// Subscribes the specified callback to the bus.
        /// </summary>
        /// <typeparam name="TMessage">Type of event to listen to.</typeparam>
        /// <param name="action">
        /// Callback that will handle any events of type <paramtyperef name="TMessage"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="action"/> is <c>null</c>.
        /// </exception>
        IDisposable Subscribe<TMessage>(Action<TMessage> action) where TMessage : class;

        /// <summary>
        /// Subscribes the specified handler to the bus.
        /// </summary>
        /// <typeparam name="TMessage">Type of event to listen to.</typeparam>
        /// <param name="handler">
        /// Handler that will handle any events of type <paramtyperef name="TDomainEvent"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="handler"/> is <c>null</c>.
        /// </exception>
        IDisposable Subscribe<TMessage>(IMessageHandler<TMessage> handler) where TMessage : class;
    }
}
