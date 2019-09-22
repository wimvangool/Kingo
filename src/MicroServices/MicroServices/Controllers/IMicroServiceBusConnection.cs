using System;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// When implemented by a class, represents a connection to a service-bus.
    /// </summary>
    public interface IMicroServiceBusConnection : IDisposable
    {
        /// <summary>
        /// Closes the connection. 
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// The connection has already been disposed.
        /// </exception>
        void Close();
    }
}
