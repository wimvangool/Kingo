using System;
using static Kingo.Ensure;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When applied to an implementation of the <see cref="IMessageHandler{TMessage}.HandleAsync"/>
    /// method, indicates that the message-handler will receive commands or events from the service bus.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class MicroServiceBusEndpointAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MicroServiceBusEndpointAttribute" /> class.
        /// </summary>
        /// <param name="types">Indicates whether this endpoint is an internal endpoint, external endpoint or both.</param>
        /// <param name="name">Represents the name of this endpoint.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="name"/> is <c>null</c>.
        /// </exception>
        public MicroServiceBusEndpointAttribute(MicroServiceBusEndpointTypes types, string name = "[handler].[message]")
        {
            Types = types;
            Name = IsNotNull(name, nameof(name));
        }

        /// <summary>
        /// Indicates whether this endpoint is an internal endpoint, external endpoint or both.
        /// </summary>
        public MicroServiceBusEndpointTypes Types
        {
            get;
        }

        /// <summary>
        /// Represents the name of the endpoint.
        /// </summary>
        public string Name
        {
            get;
        }
    }
}
