using System;
using System.Threading.Tasks;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// Represents a service-bus client that allows messages to be sent and received to and from it.
    /// </summary>
    public interface IMicroServiceBusClient : IMicroServiceBus, IMicroServiceBusConnection
    {
        /// <summary>
        /// Connects the specified <paramref name="endpoint"/> to the service-bus if it is an
        /// endpoint that is supported by this client.
        /// </summary>
        /// <param name="endpoint">The endpoint to connect.</param>
        /// <returns><c>true</c> if the endpoint was connected; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="endpoint"/> is <c>null</c>.
        /// </exception>
        Task<bool> ConnectToEndpointAsync(IMicroServiceBusEndpoint endpoint);
    }
}
