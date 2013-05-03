using System;
using System.Collections.Generic;
using System.Linq;
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
        #region [====== Nested Types ======]
        
        private sealed class CallbackHandler<TMessage> : IInternalMessageHandler<TMessage>
            where TMessage : class
        {
            private readonly Action<TMessage> _callback;

            public CallbackHandler(Action<TMessage> callback)
            {
                if (callback == null)
                {
                    throw new ArgumentNullException("callback");
                }
                _callback = callback;
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as CallbackHandler<TMessage>);
            }

            private bool Equals(CallbackHandler<TMessage> other)
            {
                if (ReferenceEquals(other, this))
                {
                    return true;
                }
                if (ReferenceEquals(other, null))
                {
                    return false;
                }
                return Equals(_callback, other._callback);
            }

            public override int GetHashCode()
            {
                return _callback.GetHashCode();
            }

            public void Handle(TMessage message)
            {
                _callback(message);
            }
        }
        
        #endregion

        private static readonly ThreadLocal<List<DomainEventBusSubscription>> _Subscriptions =
            new ThreadLocal<List<DomainEventBusSubscription>>(() => new List<DomainEventBusSubscription>());                              

        /// <summary>
        /// Subscribes the specified callback to the bus.
        /// </summary>
        /// <typeparam name="TMessage">Type of event to listen to.</typeparam>
        /// <param name="callback">
        /// Callback that will handle any events of type <paramtyperef name="TMessage"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="callback"/> is <c>null</c>.
        /// </exception>
        public static IDisposable Subscribe<TMessage>(Action<TMessage> callback)
            where TMessage : class
        {                      
            return Subscribe(new CallbackHandler<TMessage>(callback));
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
        public static IDisposable Subscribe<TMessage>(IInternalMessageHandler<TMessage> handler)
            where TMessage : class
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            return new DomainEventBusSubscription<TMessage>(_Subscriptions.Value, handler);
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
            HandleMessageWithProcessor(message);
            HandleMessageWithSubscribedHandlers(message);                       
        }
       
        private static void HandleMessageWithProcessor<TMessage>(TMessage message) where TMessage : class
        {
            var context = MessageProcessorContext.Current;
            if (context == null)
            {
                return;
            }
            var processor = context.MessageProcessor;
            if (processor == null)
            {
                return;
            }
            processor.Handle(message, true);
        }

        private static void HandleMessageWithSubscribedHandlers<TMessage>(TMessage message) where TMessage : class
        {
            if (_Subscriptions.IsValueCreated)
            {
                foreach (var subscription in _Subscriptions.Value.OfType<IInternalMessageHandler<TMessage>>())
                {
                    subscription.Handle(message);
                }
            } 
        }
    }
}
