using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kingo.Messaging
{
    /// <summary>
    /// Contains extensions for <see cref="IMessageStream" /> instances.
    /// </summary>
    public static class MessageStreamExtensions
    {
        /// <summary>
        /// Appends the specified <paramref name="message"/> to this stream and associates it with the specified <paramref name="handler"/>.
        /// </summary>     
        /// <param name="stream">The stream to append the message to.</param>   
        /// <param name="message">Message to append.</param>
        /// <param name="handler">Optional handler to associate with the message.</param>
        /// <returns>A new stream containing the appended message.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        public static IMessageStream Append<TMessage>(this IMessageStream stream, TMessage message, Action<TMessage, IMicroProcessorContext> handler) =>
            stream.Append(message, MessageHandler<TMessage>.FromDelegate(handler));

        /// <summary>
        /// Appends the specified <paramref name="message"/> to this stream and associates it with the specified <paramref name="handler"/>.
        /// </summary>        
        /// <param name="stream">The stream to append the message to.</param>
        /// <param name="message">Message to append.</param>
        /// <param name="handler">Optional handler to associate with the message.</param>
        /// <returns>A new stream containing the appended message.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        public static IMessageStream Append<TMessage>(this IMessageStream stream, TMessage message, Func<TMessage, IMicroProcessorContext, Task> handler) =>
            stream.Append(message, MessageHandler<TMessage>.FromDelegate(handler));

        /// <summary>
        /// Combines all specified <paramref name="streams"/> into one stream. All streams will be appended in order.
        /// </summary>
        /// <param name="streams">The collection of streams to combine.</param>
        /// <returns>A single stream containing all messages from the specified stream.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="streams"/> is <c>null</c>.
        /// </exception>
        public static IMessageStream Join(this IEnumerable<IMessageStream> streams) =>
            MessageStream.Concat(streams);
    }
}
