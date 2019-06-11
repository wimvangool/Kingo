using System;
using System.Collections.Generic;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represent a bus to which events can be published.
    /// </summary>
    public interface IEventBus : IReadOnlyList<object>
    {
        /// <summary>
        /// Publishes the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message to publish.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        void Publish(object message);        
    }
}
