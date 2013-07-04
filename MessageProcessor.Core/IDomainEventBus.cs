using System;

namespace YellowFlare.MessageProcessing
{
    /// <summary>
    /// Represents an internal message-bus that can be used to publish domain events and let all subscribers,
    /// direc or indirect, handle these events within the same session/transaction they were raised in.
    /// </summary> 
    public interface IDomainEventBus
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

        /// <summary>
        /// Publishes the specified event on the current thread.
        /// </summary>
        /// <typeparam name="TMessage">Type of event to raise.</typeparam>
        /// <param name="message">The event to raise.</param>        
        /// <remarks>
        /// This method will publish the specified event, letting any handlers that were registered implicitly in the
        /// currently active <see cref="MessageProcessor" /> or explicitly through one of the Subscribe-methods take
        /// care of the event. Note that all handlers that are type-compatible with the event will be called, so
        /// a handler does not have to specify exactly each type of domain event it wants to handle, but may handle
        /// a specific base- or interface-type of certain events.
        /// </remarks> 
        void Publish<TMessage>(TMessage message) where TMessage : class;
    }
}
