using System.Collections.Generic;
using System.ComponentModel.Messaging.Server;
using System.Threading;
using System.Transactions;

namespace System.ComponentModel.Messaging.Client
{
    /// <summary>
    /// Represents an event-bus for client or front-end components that can be used within a <see cref="MessageProcessor" />
    /// to listen to published events and them publish them on a UI-event bus.
    /// </summary>
    public abstract class ClientEventBus : IDomainEventBus
    {
        private readonly SynchronizationContext _context;
        private readonly ThreadLocal<ClientEventBusMessageBuffer> _messageBuffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientEventBus" /> class.
        /// </summary>
        protected ClientEventBus()
        {
            _context = SynchronizationContext.Current;
            _messageBuffer = new ThreadLocal<ClientEventBusMessageBuffer>();
        }

        /// <summary>
        /// Returns the context that is used to publish all events on.
        /// </summary>
        protected internal SynchronizationContext SynchronizationContext
        {
            get { return _context; }
        }       

        void IDomainEventBus.Publish<TMessage>(TMessage message)
        {
            if (_messageBuffer.Value == null)
            {
                _messageBuffer.Value = new ClientEventBusMessageBuffer(this, Transaction.Current);
                _messageBuffer.Value.FlushCompleted += (s, e) => _messageBuffer.Value = null;
            }
            _messageBuffer.Value.Write(message);
        }        

        /// <summary>
        /// Creates and returns a new <see cref="IConnection " /> to this bus.
        /// </summary>
        /// <param name="subscriber">
        /// The subscriber that is subscribed and unsubscribed by the connection.
        /// </param>
        /// <returns>A new connection.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="subscriber"/> is <c>null</c>.
        /// </exception>
        public IConnection Connect(object subscriber)
        {            
            return Connect(subscriber, false);
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConnection " /> to this bus.
        /// </summary>
        /// <param name="subscriber">
        /// The subscriber that is subscribed and unsubscribed by the connection.
        /// </param>
        /// <param name="open">
        /// If <c>true</c>, this method immediately opens the connection before returning.
        /// </param>
        /// <returns>A new connection.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="subscriber"/> is <c>null</c>.
        /// </exception>
        public IConnection Connect(object subscriber, bool open)
        {
            if (subscriber == null)
            {
                throw new ArgumentNullException("subscriber");
            }
            var connection = CreateConnection(subscriber);

            if (open)
            {
                connection.Open();
            }
            return connection;
        }

        private IConnection CreateConnection(object subscriber)
        {
            return new ClientEventBusConnection(this, subscriber);
        }

        /// <summary>
        /// Subscribes the specified <paramref name="subscriber"/> to this bus.
        /// </summary>
        /// <param name="subscriber">The subscriber to subscribe.</param>
        protected internal abstract void Subscribe(object subscriber);

        /// <summary>
        /// Unsubscribes the specified <paramref name="subscriber"/> from this bus.
        /// </summary>
        /// <param name="subscriber">The subscriber to unsubscribe.</param>
        protected internal abstract void Unsubscribe(object subscriber);

        /// <summary>
        /// Publishes the specified message on this bus.
        /// </summary>
        /// <param name="message">The message to publish.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        public abstract void Publish(object message);        
    }
}
