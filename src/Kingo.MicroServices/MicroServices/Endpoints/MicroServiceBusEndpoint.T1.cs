using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kingo.MicroServices.Endpoints
{
    /// <summary>
    /// When implemented, represents an endpoint that connects to a service-bus.
    /// </summary>
    /// <typeparam name="TMessageEnvelope">Type of the messages that are sent or received from the service bus.</typeparam>
    public abstract class MicroServiceBusEndpoint<TMessageEnvelope> : HostedEndpoint
    {        
        /// <summary>
        /// The serializer used to pack and unpack messages that are sent to or received
        /// from the service bus.
        /// </summary>
        protected abstract IMessageSerializer<TMessageEnvelope> Serializer
        {
            get;
        }
    }
}
