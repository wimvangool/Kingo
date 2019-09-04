using System.Threading.Tasks;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// Represents a connected client to a service-bus that allows message to be sent to it
    /// </summary>
    public interface IMicroServiceBusClient : IMicroServiceBus, IMicroServiceBusConnection
    {
        Task ConnectToEndpointAsync(IMicroServiceBusEndpoint endpoint);
    }
}
