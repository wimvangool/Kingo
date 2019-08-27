using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents a processor that can be connected to a service bus to
    /// process incoming commands and events.
    /// </summary>
    public interface IMicroServiceBusProcessor
    {
        /// <summary>
        /// Returns the service provider the processor uses to resolve its dependencies.
        /// </summary>
        IMicroProcessorServiceProvider ServiceProvider
        {
            get;
        }

        /// <summary>
        /// Configures the processor to use the specified <paramref name="user"/> for each operation as long as the
        /// returned scope is active.
        /// </summary>
        /// <param name="user">The principal to use.</param>
        /// <returns>A scope that can be disposed when the principal can be reset to its previous value.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="user"/> is <c>null</c>.
        /// </exception>
        IDisposable AssignUser(IPrincipal user);

        /// <summary>
        /// Creates and returns all endpoints that are configured to handle commands or events from a service bus.
        /// </summary>
        /// <returns>A collection of endpoints.</returns>
        IEnumerable<IHandleAsyncMethodEndpoint> CreateMethodEndpoints();
    }
}
