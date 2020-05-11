using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// When implemented, represents a client that can be used to send and receive messages from a message-broker or -bus.
    /// </summary>
    public abstract class MicroServiceBusClient : MicroServiceBus
    {
        private readonly IMicroServiceBusEndpoint[] _endpoints;

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroServiceBusClient" /> class.
        /// </summary>
        /// <param name="endpoints">
        /// A collection of endpoints configured by a <see cref="IMicroProcessor"/> to receive messages from the service-bus.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="endpoints"/> is <c>null</c>.
        /// </exception>
        protected MicroServiceBusClient(IEnumerable<IMicroServiceBusEndpoint> endpoints)
        {
            _endpoints = endpoints?.ToArray() ?? throw new ArgumentNullException(nameof(endpoints));
        }

        /// <summary>
        /// A collection of endpoints configured by a <see cref="IMicroProcessor"/> to receive messages from the service-bus.
        /// </summary>
        protected IReadOnlyCollection<IMicroServiceBusEndpoint> Endpoints =>
            _endpoints;
    }
}
