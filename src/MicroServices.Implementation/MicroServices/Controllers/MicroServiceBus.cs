using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// Serves as a base-class for all service-bus implementations.
    /// </summary>
    public abstract class MicroServiceBus : MicroServiceBusBase
    {
        private readonly IMicroServiceBusEndpoint[] _endpoints;

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroServiceBus" /> class.
        /// </summary>
        /// <param name="endpoints">
        /// A collection of endpoints configured by a <see cref="IMicroProcessor"/> to receive messages from the service-bus.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="endpoints"/> is <c>null</c>.
        /// </exception>
        protected MicroServiceBus(IEnumerable<IMicroServiceBusEndpoint> endpoints) : base(MessageDirection.Output, MessageDirection.Input)
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
