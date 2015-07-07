using System;
using System.Threading;
using System.Threading.Tasks;
using Syztem.ComponentModel.Server;
using Syztem.Threading;

namespace Syztem.ComponentModel.Client
{
    /// <summary>
    /// Represents an event-bus for client or front-end components that can be used within a <see cref="MessageProcessor" />
    /// to listen to published events and them publish them on a UI-event bus.
    /// </summary>
    public abstract class ClientEventBus : AsyncObject, IMessageHandler<object>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientEventBus" /> class.
        /// </summary>
        protected ClientEventBus() { }      
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientEventBus" /> class.
        /// </summary>
        /// <param name="synchronizationContext">The context to use to send messages to the appropriate thread.</param>
        protected ClientEventBus(SynchronizationContext synchronizationContext) : base(synchronizationContext) { }
       
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

        Task IMessageHandler<object>.HandleAsync(object message)
        {
            return AsyncMethod.RunSynchronously(() => Post(() => Publish(message)));
        }               

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
