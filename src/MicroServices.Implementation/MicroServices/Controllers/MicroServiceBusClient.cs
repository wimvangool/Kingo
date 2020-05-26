using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// When implemented, represents a <see cref="IMicroServiceBus"/>-component that can either send or
    /// receive messages from a bus.
    /// </summary>
    public abstract class MicroServiceBusClient : AsyncDisposable, IMicroServiceBus
    {
        /// <summary>
        /// Sends the specified <paramref name="messages"/>. If this client represents a sender
        /// with respect to a service-bus, then this method will send the messages to te bus. If
        /// this client represents a receiver, this method will dispatch the messages as if they
        /// were received from the bus.
        /// </summary>
        /// <param name="messages">The messages to send.</param>
        /// <exception cref="InvalidOperationException">
        /// The client is currently unable to send any messages.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messages"/> is <c>null</c>.
        /// </exception>
        public abstract Task SendAsync(IEnumerable<IMessage> messages);
    }
}
