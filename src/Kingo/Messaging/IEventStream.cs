using System;
using System.Collections.Generic;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents a stream of events.
    /// </summary>
    public interface IEventStream : IReadOnlyList<object>
    {
        /// <summary>
        /// Publishes the specified <paramref name="message"/> to the stream.
        /// </summary>
        /// <typeparam name="TEvent">Type of message to add.</typeparam>
        /// <param name="message">The message to add.</param>
        /// <exception cref="InvalidOperationException">
        /// The stream does not support publishing messages in the current context.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        void Publish<TEvent>(TEvent message);
    }
}
