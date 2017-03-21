using System;
using System.Collections.Generic;

namespace Kingo.Messaging
{
    /// <summary>
    /// Contains extensions for <see cref="IMessageStream" /> instances.
    /// </summary>
    public static class MessageStreamExtensions
    {
        /// <summary>
        /// Combines all specified <paramref name="streams"/> into one stream. All streams will be appended in order.
        /// </summary>
        /// <param name="streams">The collection of streams to combine.</param>
        /// <returns>A single stream containing all messages from the specified stream.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="streams"/> is <c>null</c>.
        /// </exception>
        public static IMessageStream Join(this IEnumerable<IMessageStream> streams)
        {
            return MessageStream.Concat(streams);
        }
    }
}
