using System;

namespace Kingo.MicroServices
{ 
    /// <summary>
    /// The <see cref="MessageBusTypes"/> can be used to configure a <see cref="IMessageHandler{TMessage}" /> that is
    /// decorated with an <see cref="MessageBusEndpointAttribute" /> to receive and process messages from specific message-buses.
    /// </summary>
    [Flags]
    public enum MessageBusTypes
    {
        /// <summary>
        /// Indicates the endpoint is disabled.
        /// </summary>
        None,

        /// <summary>
        /// Indicates the endpoint is connected to the internal <see cref="IMessageBus" />, which means it will
        /// receive events from the processor as they are published by (other) message handlers.
        /// </summary>
        InternalEventBus,

        /// <summary>
        /// Indicates the endpoint is connected to the <see cref="IMicroServiceBus" />, which means
        /// it will receive messages asynchronously and in separate transactions.
        /// </summary>
        MicroServiceBus,

        /// <summary>
        /// Indicates the endpoint is connected to all sources.
        /// </summary>
        All = InternalEventBus | MicroServiceBus
    }
}
