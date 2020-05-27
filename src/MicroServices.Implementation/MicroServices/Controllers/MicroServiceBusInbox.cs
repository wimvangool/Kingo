using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// When implemented, represents a conceptual inbox of messages of a service. An inbox allows messages to be sent to it
    /// and received from it. Messages that are retrieved from an inbox are meant to be processed by a <see cref="IMicroProcessor"/>
    /// of a service by dispatching them to one of the <see cref="IMicroServiceBusEndpoint">endpoints</see> exposed by the processor.
    /// </summary>
    public abstract class MicroServiceBusInbox : MicroServiceBus
    {
        private readonly IMicroServiceBusEndpoint[] _endpoints;

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroServiceBusInbox" /> class.
        /// </summary>
        /// <param name="endpoints">
        /// A collection of endpoints configured by a <see cref="IMicroProcessor"/> to receive messages from the service-bus.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="endpoints"/> is <c>null</c>.
        /// </exception>
        protected MicroServiceBusInbox(IEnumerable<IMicroServiceBusEndpoint> endpoints) : base(MessageDirection.Output, MessageDirection.Input)
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
