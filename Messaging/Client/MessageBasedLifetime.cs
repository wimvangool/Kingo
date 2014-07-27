using System;
using System.Collections.Generic;
using System.ComponentModel.Messaging.Server;
using System.Linq;
using System.Text;

namespace System.ComponentModel.Messaging.Client
{
    /// <summary>
    /// Represents a lifetime that connects to a <see cref="IMessageProcessorBus" /> to receive
    /// messages that (may) signal that the cached value has expired.
    /// </summary>
    public abstract class MessageBasedLifetime : QueryCacheValueLifetime
    {
        private readonly IMessageProcessorBus _bus;
        private readonly List<IConnection> _busConnections;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBasedLifetime" /> class.
        /// </summary>
        /// <param name="bus">The bus that can be connected to to receive messages.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="bus"/> is <c>null</c>.
        /// </exception>
        protected MessageBasedLifetime(IMessageProcessorBus bus)
        {
            if (bus == null)
            {
                throw new ArgumentNullException("bus");
            }
            _bus = bus;
            _busConnections = new List<IConnection>();
        }

        /// <summary>
        /// The bus that is connected to.
        /// </summary>
        protected IMessageProcessorBus Bus
        {
            get { return _bus; }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">
        /// Indicates if the method was called by the application explicitly (<c>true</c>), or by the finalizer
        /// (<c>false</c>).
        /// </param>
        /// <remarks>
        /// If <paramref name="disposing"/> is <c>true</c>, this method will dispose any managed resources immediately.
        /// Otherwise, only unmanaged resources will be released.
        /// </remarks>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var connection in _busConnections)
                {
                    connection.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Connects the specified <paramref name="handler"/> to the associated <see cref="Bus" />.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message to handle.</typeparam>
        /// <param name="handler">The handler to connect.</param>
        protected void Connect<TMessage>(IMessageHandler<TMessage> handler) where TMessage : class
        {
            _busConnections.Add(_bus.Connect(handler, true));
        }        
    }
}
