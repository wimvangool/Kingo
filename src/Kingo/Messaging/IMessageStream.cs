using System;
using System.Collections.Generic;

namespace Kingo.Messaging
{
    /// <summary>
    /// When implemented by a class, represents a read-only list of <see cref="IMessage">messages</see>.
    /// </summary>
    public interface IMessageStream : IReadOnlyList<IMessage>
    {
        /// <summary>
        /// Appends the specified stream of messages to this stream.
        /// </summary>
        /// <param name="stream">The stream to append.</param>
        /// <returns>A new stream, containing the messages of both the current and the specified stream.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream"/> is <c>null</c>.
        /// </exception>
        IMessageStream Append(IMessageStream stream);        

        /// <summary>
        /// If not <c>null</c>, lets the specified <paramref name="handler"/> handle all messages of this stream.
        /// </summary>
        /// <param name="handler">A handler of messages.</param>        
        void Accept(IMessageHandler handler);
    }
}
