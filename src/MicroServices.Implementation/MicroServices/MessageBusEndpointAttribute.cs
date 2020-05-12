using System;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When applied to an implementation of the <see cref="IMessageHandler{TMessage}.HandleAsync"/>
    /// method, indicates that the message-handler will receive commands or events from the service bus.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class MessageBusEndpointAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBusEndpointAttribute" /> class.
        /// </summary>
        /// <param name="types">Indicates which message-bus types this endpoint is connected to.</param>
        /// <param name="nameFormat">A special format-string that will be used to dynamically resolve the name of this endpoint.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="nameFormat"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="FormatException">
        /// <paramref name="nameFormat"/> is not a valid name-format.
        /// </exception>
        public MessageBusEndpointAttribute(MessageBusTypes types, string nameFormat = "[service].[handler].[message]")
        {
            Types = types;
            NameFormat = MicroServiceBusEndpointNameFormat.Parse(nameFormat);
        }

        /// <summary>
        /// Indicates whether this endpoint is an internal endpoint, external endpoint or both.
        /// </summary>
        public MessageBusTypes Types
        {
            get;
        }

        /// <summary>
        /// Returns the name-format of the endpoint.
        /// </summary>
        public MicroServiceBusEndpointNameFormat NameFormat
        {
            get;
        }
    }
}
