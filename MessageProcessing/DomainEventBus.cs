using System;
using System.Collections.Generic;
using System.Threading;

namespace YellowFlare.MessageProcessing
{
    /// <summary>
    /// Represents an internal message-bus that can be used to publish events from a domain to all listeners in
    /// the application-layer (or possibly other components).
    /// </summary>
    /// <remarks>
    /// <para>
    /// Inspired by the proposed design of Udi Dahan, the primary goal of the <see cref="DomainEventBus" /> is to
    /// let the domain communicate with other components in the system or application in a loosely coupled fashion,
    /// but within a single transaction if that is required. This allows the domain-layer to be completely
    /// independent of any other layer or component in the system.
    /// </para>       
    /// </remarks>
    public static class DomainEventBus
    {        
        private static readonly ThreadLocal<List<DomainEventBusSubscription>> _Subscriptions =
            new ThreadLocal<List<DomainEventBusSubscription>>(() => new List<DomainEventBusSubscription>());                              

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
        public static IDisposable Subscribe<TMessage>(Action<TMessage> action) where TMessage : class
        {
            return new DomainEventBusSubscriptionForAction<TMessage>(_Subscriptions.Value, action);
        }

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
        public static IDisposable Subscribe<TMessage>(IMessageHandler<TMessage> handler) where TMessage : class
        {            
            return new DomainEventBusSubscriptionForInterface<TMessage>(_Subscriptions.Value, handler);
        }
        
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
        public static void Publish<TMessage>(TMessage message) where TMessage : class
        {
            MessageProcessor processor;

            if (MessageProcessor.TryGetCurrent(out processor))
            {
                HandleMessageWithRegisteredHandlers(processor, message);
                HandleMessageWithSubscribedHandlers(processor, message);    
            }            
        }

        private static void HandleMessageWithRegisteredHandlers<TMessage>(MessageProcessor processor, TMessage message) where TMessage : class
        {
            processor.Handle(message, MessageSources.DomainEventBus);
        }              

        private static void HandleMessageWithSubscribedHandlers<TMessage>(MessageProcessor processor, TMessage message) where TMessage : class
        {
            if (_Subscriptions.IsValueCreated)
            {
                foreach (var subscription in _Subscriptions.Value)
                {
                    subscription.Handle(processor, message);
                }
            } 
        }
    }
}
