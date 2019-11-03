namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// When implemented by a class, represents a component that can resolve the name of an endpoint.
    /// </summary>
    public interface IEndpointNameResolver
    {
        /// <summary>
        /// Resolves the name of the specified <paramref name="endpoint"/>. This name will be used
        /// to name/identify the name of the queue from which the messages are pulled for this endpoint.
        /// </summary>
        /// <param name="endpoint">An endpoint for which the name must be resolved.</param>
        /// <returns></returns>
        string ResolveName(IMicroServiceBusEndpoint endpoint);
    }
}
