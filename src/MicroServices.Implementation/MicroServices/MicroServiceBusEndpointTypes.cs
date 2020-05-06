using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices
{ 
    /// <summary>
    /// The <see cref="MicroServiceBusEndpointTypes"/> can be used to indicate whether an endpoint
    /// represents an internal endpoint, external endpoint or both.
    /// </summary>
    [Flags]
    public enum MicroServiceBusEndpointTypes
    {
        /// <summary>
        /// Indicates the endpoint is disabled.
        /// </summary>
        None,

        /// <summary>
        /// Indicates the endpoint is an internal endpoint, which means it will receive and handle
        /// messages inside the same logical transaction as they were sent or published.
        /// </summary>
        Internal,

        /// <summary>
        /// Indicates the endpoint is an external endpoint, which means it will receive and handle
        /// messages from an external service-bus.
        /// </summary>
        External,

        /// <summary>
        /// Indicates the endpoint is both an internal and external endpoint.
        /// </summary>
        All = Internal | External
    }
}
