using System;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// Represents a set of modes of a <see cref="MicroServiceBusController"/> that determine
    /// whether the controller is able to send and/or receive messages from the bus.
    /// </summary>
    [Flags]
    public enum MicroServiceBusModes
    {
        /// <summary>
        /// Indicates the controller is neither sending nor receiving any messages.
        /// </summary>
        Disabled,

        /// <summary>
        /// Indicates the controller is able to send messages to the bus.
        /// </summary>
        Send,

        /// <summary>
        /// Indicates the controller is able to receive messages from the bus.
        /// </summary>
        Receive,

        /// <summary>
        /// Indicates the controller is both able to send and receive messages from the bus.
        /// </summary>
        SendAndReceive = Send | Receive
    }
}
