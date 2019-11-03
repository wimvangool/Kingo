﻿using System;
using System.Collections.Generic;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represent a bus to which events can be published.
    /// </summary>
    public interface IMessageBus : IReadOnlyList<MessageToDispatch>
    {
        /// <summary>
        /// Schedules the specified <paramref name="command"/> to be sent on the service-bus.
        /// </summary>
        /// <param name="command">The command to send.</param>
        /// <param name="deliveryTime">
        /// If specified, indicates at what time the command should be sent on the service-bus.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="command"/> is <c>null</c>.
        /// </exception>
        void SendCommand(object command, DateTimeOffset? deliveryTime = null);

        /// <summary>
        /// Schedules the specified <paramref name="event"/> to be published on the service-bus.
        /// </summary>
        /// <param name="event">The event to publish.</param>
        /// <param name="deliveryTime">
        /// If specified, indicates at what time the event should be published on the service-bus.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="event"/> is <c>null</c>.
        /// </exception>
        void PublishEvent(object @event, DateTimeOffset? deliveryTime = null);        
    }
}