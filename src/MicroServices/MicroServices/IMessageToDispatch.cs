using System;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents a message that is to be delivered by means of a service-bus.
    /// </summary>
    public interface IMessageToDispatch : IMessage
    {
        /// <summary>
        /// If specified, indicates at what (UTC) time the message should be sent or published on the service-bus.
        /// </summary>
        DateTimeOffset? DeliveryTimeUtc
        {
            get;
        }
    }
}
