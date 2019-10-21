using System;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When applied to an implementation of the <see cref="IMessageHandler{TMessage}.HandleAsync"/>
    /// method, indicates that the message-handler will receive events from the internal processor
    /// bus.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class InternalEventBusEndpointAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InternalEventBusEndpointAttribute" /> class.
        /// </summary>
        /// <param name="acceptScheduledEvents">Indicates whether or not the handler accepts events that were scheduled.</param>
        public InternalEventBusEndpointAttribute(bool acceptScheduledEvents = false)
        {
            AcceptScheduledEvents = acceptScheduledEvents;
        }

        /// <summary>
        /// Indicates whether or not the handler accepts events that were scheduled.
        /// </summary>
        public bool AcceptScheduledEvents
        {
            get;
        }
    }
}
