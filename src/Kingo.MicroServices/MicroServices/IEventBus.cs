using System;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represent a bus to which events can be published.
    /// </summary>
    public interface IEventBus
    {
        /// <summary>
        /// Publishes the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message to publish.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        void Publish(object message);

        /// <summary>
        /// Creates and returns a <see cref="MessageStream"/> that contains all events that has been published to this bus.
        /// </summary>
        /// <returns>A new stream.</returns>
        MessageStream ToStream();
    }
}
